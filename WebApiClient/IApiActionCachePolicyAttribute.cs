using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 定义ApiAction缓存的策略
    /// </summary>
    public interface IApiActionCachePolicyAttribute : IApiActionCacheAttribute
    {
        /// <summary>
        /// 指示是否需要读取缓存
        /// 返回false则直接进入接口请求
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        bool NeedReadCache(ApiActionContext context);

        /// <summary>
        /// 指示是否需要写入缓存
        /// 返回false则不会将请求的响应写入缓存中
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        bool NeedWriteCache(ApiActionContext context);
    }
}
