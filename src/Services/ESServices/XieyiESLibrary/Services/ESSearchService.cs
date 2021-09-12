using Microsoft.Extensions.Logging;
using Nest;
using XieyiESLibrary.Interfaces;

namespace XieyiESLibrary.Services
{
    public sealed class ESSearchService : IESSearch
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<ESSearchService> _logger;

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