using System;
using System.Threading.Tasks;
using WebApiClientCore.ResponseCaches;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义ApiAction缓存修饰特性的行为
    /// </summary>
    public interface IApiCacheAttribute
    {
        /// <summary>
        /// 获取缓存的时间戳
        /// </summary>
        TimeSpan Expiration { get; }

        /// <summary>
        /// 返回读取缓存的策略       
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        CachePolicy GetReadPolicy(ApiRequestContext context);

        /// <summary>
        /// 返回写入缓存的策略
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        CachePolicy GetWritePolicy(ApiRequestContext context);

        /// <summary>
        /// 获取缓存提供者
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IResponseCacheProvider? GetCacheProvider(ApiRequestContext context);

        /// <summary>
        /// 返回请求对应的缓存的键
        /// 该键用于读取或写入缓存到缓存提供者
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        Task<string> GetCacheKeyAsync(ApiRequestContext context);
    }
}
