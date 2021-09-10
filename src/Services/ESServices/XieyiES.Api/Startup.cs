using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Serilog;
using XieyiES.Api.Extensions;
using XieyiESLibrary;

namespace XieyiES.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    { Title = "ElasticSearch Api", Version = "v1", Description = "Docs for elasticSearch" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSingleton(Log.Logger);
            services.AddESServiceInDI(options =>
            {
                options.Urls = Configuration["ElasticSearch:Uri"];
                options.UserName = Configuration["ElasticSearch:UserName"];
                options.Password = Configuration["ElasticSearch:Password"];
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ElasticSearch v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseCustomExceptionHandler();

            app.UseHttpsRedirection();

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}