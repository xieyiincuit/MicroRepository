namespace XieyiESLibrary.Interfaces
{
    /// <summary>
    ///     Search
    /// </summary>
    public interface IESSearch
    {
        IESQueryable<T> Queryable<T>() where T : class;
    }
}