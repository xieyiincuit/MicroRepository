using System;
using XieyiESLibrary.Entity;

namespace XieyiES.Api.Domain
{
    public class RegistryRecord : ESBaseEntity
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public DateTime RegistryTime { get; set; }
    }
}