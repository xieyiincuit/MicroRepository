using System;
using XieyiESLibrary.Entity;

namespace XieyiES.Api.Model
{
    public class RegistryRecord : ESBaseEntity
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public DateTimeOffset RegistryTime => DateTimeOffset.Now;

        public string RegistryArea { get; set; }
    }
}