using System;

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
        public string Id { get; set; } = Guid.NewGuid().ToString();
    }
}