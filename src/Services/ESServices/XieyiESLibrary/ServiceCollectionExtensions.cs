using System;
using Microsoft.Extensions.DependencyInjection;
using XieyiESLibrary.Config;
using XieyiESLibrary.Provider;
using XieyiESLibrary.Provider.Base;

namespace XieyiESLibrary
{
    public static class ServiceCollectionExtensions
    {
        public static void AddESServiceInDI(this IServiceCollection services, Action<ESConfig> setupAction)
        {
            if (setupAction == null)
                throw new ArgumentNullException(nameof(setupAction), "调用 ElasticSearch 配置时出错，未传入配置信息。");

            services.Configure(setupAction);

            services.AddSingleton<IESClientProvider, ESClientProvider>();
            services.AddScoped<IESRepository, ESRepository>();
        }
    }
}