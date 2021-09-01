using Microsoft.Extensions.Logging;
using Nest;
using XieyiESLibrary.Interfaces;

namespace XieyiESLibrary.Services
{
    public sealed class ESSearchService : IESSearch
    {
        private readonly ILogger<ESSearchService> _logger;
        private readonly IElasticClient _elasticClient;

        public ESSearchService(IESClientProvider esClientProvider, ILogger<ESSearchService> logger)
        {
            _logger = logger;
            _elasticClient = esClientProvider.ElasticClient;
        }

        public IESQueryable<T> Queryable<T>() where T : class
        {
            return new ESQueryableService<T>(_elasticClient, _logger);
        }
    }
}