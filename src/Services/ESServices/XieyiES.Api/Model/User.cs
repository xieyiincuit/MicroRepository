using System;
using XieyiESLibrary.Entity;

namespace XieyiES.Api.Model
{
    public class User : ESBaseEntity
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public DateTimeOffset CreateTime => DateTimeOffset.Now;

        public decimal Money { get; set; }
    }
}