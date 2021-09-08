using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nest;
using XieyiESLibrary.Entity;
using XieyiESLibrary.Entity.Mapping;
using XieyiESLibrary.ExpressionsToQuery;
using XieyiESLibrary.ExpressionsToQuery.Common;
using XieyiESLibrary.Interfaces;

namespace XieyiESLibrary.Services
{
    public class ESQueryableService<T> : IESQueryable<T> where T :class
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<ESSearchService> _logger;
        private readonly ISearchRequest _request;
        private readonly MappingIndex _mappingIndex;
        private readonly QueryBuilder<T> _queryBuilder;
        private long _totalNumber;

        public ESQueryableService(IElasticClient client, ILogger<ESSearchService> logger)
        {
            _queryBuilder = new QueryBuilder<T>();
            _mappingIndex = _queryBuilder.GetMappingIndex();
            _request = new SearchRequest(_mappingIndex.IndexName.ToLower())
            {
                Size = 10000
            };
            _elasticClient = client;
            _logger = logger;
        }

        public async Task<T> FirstAsync()
        {
            try
            {
                var result = await _ToListAsync<T>();
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Message:{ex.Message}{Environment.NewLine}Stack:{ex.StackTrace}");
                return Activator.CreateInstance<T>();
            }
        }

        public List<T> ToList()
        {
            try
            {
                return _ToList<T>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Message:{ex.Message}{Environment.NewLine}Stack:{ex.StackTrace}");
                return Activator.CreateInstance<List<T>>();
            }
        }

        public async Task<List<T>> ToListAsync()
        {
            try
            {
                return await _ToListAsync<T>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Message:{ex.Message}{Environment.NewLine}Stack:{ex.StackTrace}");
                return Activator.CreateInstance<List<T>>();
            }
        }

        public async Task<List<T>> ToPageListAsync(int pageIndex, int pageSize)
        {
            try
            {
                _request.From = ((pageIndex < 1 ? 1 : pageIndex) - 1) * pageSize;
                _request.Size = pageSize;
                return await _ToListAsync<T>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Message:{ex.Message}{Environment.NewLine}Stack:{ex.StackTrace}");
                return Activator.CreateInstance<List<T>>();
            }
           
        }

        public List<T> ToPageList(int pageIndex, int pageSize, out long totalNumber)
        {
            try
            {
                var list = ToPageListAsync(pageIndex, pageSize).GetAwaiter().GetResult();
                totalNumber = _totalNumber;
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Message:{ex.Message}{Environment.NewLine}Stack:{ex.StackTrace}");
                totalNumber = 0;
                return Activator.CreateInstance<List<T>>();
            }
            
        }

        public IESQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            _request.Query = _queryBuilder.GetQueryContainer(expression);
            return this;
        }

        public IESQueryable<T> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }

        public IESQueryable<T> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }

        private List<TResult> _ToList<TResult>() where TResult : class
        {
            var response = _elasticClient.Search<TResult>(_request);

            if (!response.IsValid)
                throw new Exception($"Search index:[{typeof(TResult).Name.ToLower()}] failed -> Message:{response.OriginalException}");

            _totalNumber = response.Total;
            return response.Documents.ToList();
        }

        private async Task<List<TResult>> _ToListAsync<TResult>() where TResult : class
        {
            var response = await _elasticClient.SearchAsync<TResult>(_request);

            if (!response.IsValid)
                throw new Exception($"SearchAsync index:[{typeof(TResult).Name.ToLower()}] failed -> Message:{response.OriginalException}");

            _totalNumber = response.Total;
            return response.Documents.ToList();
        }

        private void _GroupBy(Expression expression)
        {
            var propertyName = ReflectionExtensionHelper.GetProperty(expression as LambdaExpression).Name;
            propertyName = _mappingIndex.Columns.FirstOrDefault(x => x.PropertyName == propertyName)?.SearchName ?? propertyName;
            _request.Aggregations = new TermsAggregation(propertyName)
            {
                Field = propertyName,
                Size = 1000
            };
        }

        private void _OrderBy(Expression expression, OrderByType type = OrderByType.Asc)
        {
            var propertyName = ReflectionExtensionHelper.GetProperty(expression as LambdaExpression).Name;
            propertyName = _mappingIndex.Columns.FirstOrDefault(x => x.PropertyName == propertyName)?.SearchName ?? propertyName;
            _request.Sort = new ISort[]
            {
                new FieldSort
                {
                    Field = propertyName,
                    Order = type == OrderByType.Asc ? SortOrder.Ascending : SortOrder.Descending
                }
            };
        }
    }
}