namespace XieyiRedisLibrary.Config
{
    public class RedisConfig
    {
        /// <summary>
        ///     redis ip
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        ///     additional settings split with ,
        /// </summary>
        public string OptionalSettings { get; set; }

        /// <summary>
        ///     clintName in redis 6
        /// </summary>
        public string ClintName { get; set; }

        /// <summary>
        ///     password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     which db you wanna connect
        /// </summary>
        public int DbNum { get; set; }

        /// <summary>
        ///     keyPrefix
        /// </summary>
        public string KeyPrefix { get; set; }

    }
}
