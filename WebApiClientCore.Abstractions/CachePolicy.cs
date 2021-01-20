namespace WebApiClientCore
{
    /// <summary>
    /// 缓存策略
    /// </summary>
    public enum CachePolicy : byte
    {
        /// <summary>
        /// 尝试使用缓存
        /// </summary>
        Include,

        /// <summary>
        /// 忽略并跳过缓存
        /// </summary>
        Ignore
    }
}
