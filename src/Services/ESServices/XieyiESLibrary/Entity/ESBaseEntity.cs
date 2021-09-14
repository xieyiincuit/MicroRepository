using System;
using Nest;

namespace XieyiESLibrary.Entity
{
    /// <summary>
    ///     Base Entity
    /// </summary>
    public class ESBaseEntity
    {
        /// <summary>
        ///     Id (default is guid)
        /// </summary>
        [Keyword(Name = "id")]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
    }
}