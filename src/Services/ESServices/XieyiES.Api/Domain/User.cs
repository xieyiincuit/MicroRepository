using System;
using XieyiESLibrary.Entity;

namespace XieyiES.Api.Domain
{
    /// <summary>
    ///     test entity
    /// </summary>
    public class UserWallet : ESBaseEntity
    {
        /// <summary>
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// </summary>
        public decimal Money { get; set; }
    }

    /// <summary>
    ///     test entity
    /// </summary>
    public class Manager
    {
        /// <summary>
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// </summary>
        public decimal Money { get; set; }
    }
}