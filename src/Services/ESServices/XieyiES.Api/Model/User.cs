using System;
using Nest;
using XieyiESLibrary.Entity;

namespace XieyiES.Api.Model
{
    [ElasticsearchType(RelationName = "user")]
    public class User : ESBaseEntity
    {
        [Keyword(Name = "user_id")]
        public string UserId { get; set; }

        [Text(Name = "user_name")]
        public string UserName { get; set; }

        [Date(Name = "insert_time")]
        public DateTimeOffset CreateTime => DateTimeOffset.Now.LocalDateTime;

        [Number(Name = "money")]
        public decimal Money { get; set; }
    }
}