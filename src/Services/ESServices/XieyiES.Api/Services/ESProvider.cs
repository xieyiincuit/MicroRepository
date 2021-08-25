using System;
using System.Linq;
using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Nest;
using XieyiES.Api.Interfaces;

namespace XieyiES.Api.Services
{
    /// <summary>
    /// ElasticSearch初始化
    /// </summary>
    public class ESProvider : IESProvider
    {
        public ElasticClient ElasticClient { get; set; }
        public ESProvider(IConfiguration configuration)
        {
            
            var uri = configuration["ElasticSearchSettings:ConnectionUrl"].Split(',').ToList()
                .ConvertAll(x => new Uri(x)); // 配置节点地址
            var connectionPool = new StaticConnectionPool(uri); //配置请求池
            var settings = new ConnectionSettings(connectionPool).DefaultIndex("Student").RequestTimeout(TimeSpan.FromSeconds(30)); //配置请求参数

            ElasticClient = new ElasticClient(settings);

        }
    }
}