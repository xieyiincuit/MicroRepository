using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Serilog;
using XieyiES.Api.Model;
using XieyiESLibrary.Interfaces;

namespace XieyiES.Api.Controllers
{
    [ApiController]
    [Route("api/v1/student/record")]
    public class UserLoginController : ControllerBase
    {
        /// <summary>
        ///     NEST Client
        /// </summary>
        private readonly IElasticClient _elasticClient;

        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IESSearch _elasticSearch;

        public UserLoginController(IESClientProvider elasticClient, ILogger logger, IMapper mapper, IESSearch elasticSearch)
        {
            _elasticClient = elasticClient.ElasticClient ?? throw new ArgumentNullException(nameof(elasticClient.ElasticClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _elasticSearch = elasticSearch;
        }

        /// <summary>
        ///     新增测试数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> InsertUserLoginInfo()
        {
            var loginRecords = new List<UserLogin>
            {
                new UserLogin
                {
                    Id = Guid.NewGuid().ToString(),
                    NickName = "张三",
                    CreateTime = DateTime.Now,
                    College = "001",
                    OnLineTime = 123
                },

                new UserLogin
                {
                    Id = Guid.NewGuid().ToString(),
                    NickName = "李四",
                    CreateTime = DateTime.Now,
                    College = "001",
                    OnLineTime = 222
                },

                new UserLogin
                {
                    Id = Guid.NewGuid().ToString(),
                    NickName = "王五",
                    CreateTime = DateTime.Now.AddDays(4),
                    College = "002",
                    OnLineTime = 444
                }
            };

            var response = await _elasticClient.IndexManyAsync(loginRecords, nameof(UserLogin).ToLower());
            if (!response.IsValid)
            {
                return BadRequest();
            }
            return Ok(loginRecords);
        }


        /// <summary>
        ///     获取日期时间段内的记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserLoginInfo()
        {
            //var response = await _elasticClient.SearchAsync<UserLogin>(s=>s.Index("userlogin"));
            var response = await _elasticClient.SearchAsync<UserLogin>(s => s.Query(q => q
                    .DateRange(r => r
                        .Field(f => f.CreateTime)
                        .GreaterThanOrEquals(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day))
                        .LessThan(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(7).Day))))
                .Aggregations(aggs => aggs.Terms("group_of_date", group => group.Field(x => x.CreateTime)
                    .Aggregations(childAggs=> childAggs.Sum("sum_of_online", sum=>sum.Field(x=>x.OnLineTime)))))
                .Index("userlogin"));

            if (response == null)
            {
                return NotFound();
            }

            var aggsResult = response.Aggregations.Terms("group_of_date");
            var onlineTimeDic = new Dictionary<string, double?>();
            foreach (var item in aggsResult.Buckets)
            {
                var childValue = (ValueAggregate)item.Values.FirstOrDefault();
                onlineTimeDic.Add(item.Key, childValue.Value);
            }
            return Ok(onlineTimeDic);
        }


        /// <summary>
        ///     按学院分组 再聚合在线时间
        /// </summary>
        /// <returns></returns>
        [HttpGet("group")]
        public async Task<IActionResult> GetUserLoginInfoGroupCollege()
        {
            //这里的College Type是string，string映射包含了全文索引或精确值 我们这里使用精确值 给field填上 keyword后缀
            var searchResponse = await _elasticClient.SearchAsync<UserLogin>(s => s
                .Aggregations(aggs => aggs.Terms("group_of_college", group => group.Field(x => x.College.Suffix("keyword"))
                    .Aggregations(childAggs => childAggs.Sum("sum_of_online", sum => sum.Field(x => x.OnLineTime)))))
                .Index(nameof(UserLogin).ToLower()));

            if (searchResponse == null)
            {
                return NotFound();
            }

            var aggsResult = searchResponse.Aggregations.Terms("group_of_college");
            var onlineTimeDic = new Dictionary<string, double?>();
            foreach (var item in aggsResult.Buckets)
            {
                var childValue = (ValueAggregate)item.Values.FirstOrDefault();
                onlineTimeDic.Add(item.Key, childValue.Value);
            }
            return Ok(onlineTimeDic);
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetUserById(string name)
        {
            var loginRecord = await _elasticSearch.Queryable<UserLogin>().Where(x => x.NickName == name).FirstAsync();
            return Ok(loginRecord);
        }

    }
}