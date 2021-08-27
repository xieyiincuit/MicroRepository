using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace XieyiES.Api
{
    
    public class Program
    {
        public static void Main(string[] args)
        {
            using var logFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole()
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("XieyiES.Api.Program", LogLevel.Debug); // ������Info �������־�������ӡ������
            });
            var logger = logFactory.CreateLogger<Program>();

            //var logger = host.Services.GetRequiredService<ILogger<Program>>(); //�ɴ�DI��ֱ�ӻ�ȡ
            try
            {
                logger.LogInformation("XieyiES Service is staring...");
                CreateHostBuilder(args).Build().Run();
                logger.LogInformation("XieyiES Service is ending...");
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex,"XieyiES start failed, ES Service will dead!!!");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}