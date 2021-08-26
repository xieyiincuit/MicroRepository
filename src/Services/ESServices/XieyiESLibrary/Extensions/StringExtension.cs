namespace XieyiESLibrary.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// 保证indexName合理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetIndex<T>(this string index) where T : class 
            => string.IsNullOrWhiteSpace(index) ? typeof(T).Name.ToLower() : index;

        /// <summary>
        /// 首字母小写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToFirstLower(this string str)
        {
            return str.Substring(0, 1).ToLower() + str[1..];
        }
    }
}
