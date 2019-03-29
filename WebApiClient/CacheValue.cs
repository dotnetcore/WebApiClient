namespace WebApiClient
{
    /// <summary>
    /// 表示缓存的项
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CacheValue<T>
    {
        /// <summary>
        /// 获取或设置缓存项
        /// </summary>
        public T Item { get; set; }
    }
}
