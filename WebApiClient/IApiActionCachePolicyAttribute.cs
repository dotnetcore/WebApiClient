using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 定义ApiAction缓存的策略
    /// </summary>
    public interface IApiActionCachePolicyAttribute : IApiActionCacheAttribute
    {
        /// <summary>
        /// 返回读取缓存的策略       
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        CachePolicy GetReadPolicy(ApiActionContext context);

        /// <summary>
        /// 返回写入缓存的策略
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        CachePolicy GetWritePolicy(ApiActionContext context);
    }

    /// <summary>
    /// 缓存策略
    /// </summary>
    public enum CachePolicy
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
