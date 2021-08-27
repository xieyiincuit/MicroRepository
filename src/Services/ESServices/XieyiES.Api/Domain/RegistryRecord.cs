using System;
using XieyiESLibrary.Entity;

namespace XieyiES.Api.Domain
{
    /// <summary>
    ///     test entity
    /// </summary>
    public class RegistryRecord : ESBaseEntity
    {
        /// <summary>
        ///     用户Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        ///     用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     注册时间
        /// </summary>
        public DateTime RegistryTime { get; set; } = DateTime.Now;
    }
}