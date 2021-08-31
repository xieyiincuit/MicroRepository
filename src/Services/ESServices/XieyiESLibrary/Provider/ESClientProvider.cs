using System;
using System.Linq;
using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using XieyiESLibrary.Config;
using XieyiESLibrary.Provider.Base;

namespace XieyiESLibrary.Provider
{
    public class ESClientProvider : IESClientProvider
    {
        public ESClientProvider(IOptions<ESConfig> esConfig, ILogger<ESClientProvider> logger)
        {
            try
            {
                logger.LogInformation("Start to Initialize ESClient");
                var uris = esConfig.Value.Uris;
                if (uris == null || uris.Count < 1) throw new Exception("urls can not be null");

                //根据uri的个数选择不同的连接方式
                ConnectionSettings connectionSetting;
                if (uris.Count == 1)
                {
                    var uri = uris.First();
                    connectionSetting = new ConnectionSettings(uri).DeadTimeout(TimeSpan.FromSeconds(30)).DefaultIndex("");
                }
                else
                {
                    var connectionPool = new SniffingConnectionPool(uris);
                    connectionSetting = new ConnectionSettings(connectionPool).DeadTimeout(TimeSpan.FromSeconds(30)).DefaultIndex("");
                }

                //如果初始化了Name & Password 考虑使用验证
                if (!string.IsNullOrWhiteSpace(esConfig.Value.UserName) &&
                    !string.IsNullOrWhiteSpace(esConfig.Value.Password))
                {
                    connectionSetting.BasicAuthentication(esConfig.Value.UserName, esConfig.Value.Password);
                    logger.LogInformation($"Authentication successfully -> UserName :[{esConfig.Value.UserName}]");
                }

                ElasticClient = new ElasticClient(connectionSetting);
                logger.LogInformation("Initialize ESClient Success");
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "ElasticSearchClient Initialized failed.");
            }
        }

        public ElasticClient ElasticClient { get; }
    }
}