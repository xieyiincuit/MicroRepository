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
            => string.IsNullOrWhiteSpace(index) ? nameof(T).ToLower() : index;
    }
}
