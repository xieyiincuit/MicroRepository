using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nest;
using XieyiES.Api.Model;
using XieyiESLibrary.Interfaces;

namespace XieyiES.Api.Controllers
{
    [ApiController]
    [Route("api/v1/statistics/coursejoin")]
    public class CourseJoinController : ControllerBase
    {
        private readonly IElasticClient _elasticClient;
        private readonly IESRepository _esRepository;

        public CourseJoinController(IESRepository esRepository, IESClientProvider esClientProvider)
        {
            _esRepository = esRepository;
            _elasticClient = esClientProvider.ElasticClient;
        }

        /// <summary>
        ///     插入测试数据
        /// </summary>
        /// <returns></returns>
        [HttpPost("testdata")]
        public async Task<IActionResult> InsertTestDataAsync()
        {
            var courseRecords = new List<CourseJoinRecord>
            {
                new()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    CourseId = "001",
                    ChapterId = "001-chapter1",
                    UserCode = "xieyi",
                    CourseType = "1",
                    LearningStatus = 2
                },
                new()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    CourseId = "001",
                    ChapterId = "001-chapter2",
                    UserCode = "xieyi",
                    CourseType = "1",
                    LearningStatus = 1
                },
                new()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    CourseId = "002",
                    ChapterId = "002-chapter1",
                    UserCode = "xieyi",
                    CourseType = "2",
                    LearningStatus = 2
                },
                new()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    CourseId = "003",
                    UserCode = "xieyi",
                    CourseType = "2",
                    LearningStatus = 2
                },
                new()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    CourseId = "006",
                    UserCode = "zhangliang",
                    CourseType = "1",
                    LearningStatus = 1
                },
                new()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    CourseId = "007",
                    UserCode = "zhangliang",
                    CourseType = "1",
                    LearningStatus = 2
                },
                new()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    CourseId = "008",
                    UserCode = "zhangliang",
                    CourseType = "2",
                    LearningStatus = 2
                }
            };
            var response = await _esRepository.InsertRangeAsync(courseRecords, "coursejoinrecord");

            if (!response) return BadRequest();

            return Ok(courseRecords);
        }

        /// <summary>
        ///     删除索引
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteIndexAsync()
        {
            var exist = await _esRepository.IndexExistsAsync(nameof(CourseJoinRecord).ToLower());
            if (!exist) return BadRequest("index don't exist in node");

            await _esRepository.DeleteIndexAsync<CourseJoinRecord>();
            return Ok("index has been deleted");
        }

        /// <summary>
        ///     获取学生参加课程类别信息
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        [HttpGet("group/coursetype")]
        public async Task<IActionResult> GetCourseRecordGroupByUserAsync([FromQuery] string userCode)
        {
            var indexName = nameof(CourseJoinRecord).ToLower();
            var courseTypeDic = new Dictionary<string, string>
            {
                { "0", "微课" },
                { "1", "Mooc" },
                { "2", "Spoc" }
            };
            if (!string.IsNullOrWhiteSpace(userCode))
            {
                var courseJoinRecord = await _elasticClient.SearchAsync<CourseJoinRecord>(s => s.Index(indexName)
                    .Query(q => q
                        .Bool(b => b.Must(m => m.Term(t => t.Field(f => f.UserCode).Value(userCode)))))
                    .Aggregations(aggs => aggs.Terms("group_by_coursetype", t => t
                        .Field(f => f.CourseType.Suffix("keyword"))
                        .Aggregations(childAggs => childAggs.Cardinality("distinct_by_courseid",
                            card => card.Field(x => x.CourseId.Suffix("keyword")))))));

                var aggsResult = courseJoinRecord.Aggregations.Terms("group_by_coursetype");
                var courseJoinDic = new Dictionary<string, double?>();
                foreach (var item in aggsResult.Buckets)
                {
                    if (courseTypeDic.ContainsKey(item.Key)) item.Key = courseTypeDic[item.Key];
                    var childValue = (ValueAggregate)item.Values.FirstOrDefault();
                    courseJoinDic.Add(item.Key, childValue.Value);
                }

                return Ok(courseJoinDic);
            }

            var courseJoinRecords = await _elasticClient.SearchAsync<CourseJoinRecord>(s => s.Index(indexName));

            if (!courseJoinRecords.IsValid) return NotFound();
            return Ok(courseJoinRecords.Documents);
        }

        /// <summary>
        ///     获取学习学习进度信息
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        [HttpGet("group/learingstatus")]
        public async Task<IActionResult> GetCourseLearningStatusByUserAsync([FromQuery] string userCode)
        {
            var indexName = nameof(CourseJoinRecord).ToLower();
            var learningSituationDic = new Dictionary<string, int>
            {
                { "学习中", 0 },
                { "已完成", 0 }
            };


            if (!string.IsNullOrWhiteSpace(userCode))
            {
                var learningSituationRecord = await _elasticClient.SearchAsync<CourseJoinRecord>(s => s.Index(indexName)
                    .Query(q => q.Bool(b => b.Must(m => m.Term(t => t.Field(f => f.UserCode).Value(userCode))))));

                var recordGroupByCourseId =
                    from record in learningSituationRecord.Documents
                    group record by record.CourseId
                    into courseIdGroup
                    orderby courseIdGroup.Key
                    select courseIdGroup;

                foreach (var idGroup in recordGroupByCourseId)
                {
                    if (idGroup.Any(x => x.LearningStatus == 1))
                    {
                        learningSituationDic["学习中"]++;
                        continue;
                    }

                    if (idGroup.Any(x => x.LearningStatus == 2)) learningSituationDic["已完成"]++;
                }

                return Ok(learningSituationDic);
            }

            var courseJoinRecords = await _elasticClient.SearchAsync<CourseJoinRecord>(s => s.Index(indexName));

            if (!courseJoinRecords.IsValid) return NotFound();
            return Ok(courseJoinRecords.Documents);
        }
    }
}