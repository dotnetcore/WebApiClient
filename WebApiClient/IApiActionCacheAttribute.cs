using System;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 定义ApiAction缓存修饰特性的行为
    /// </summary>
    public interface IApiActionCacheAttribute
    {
        /// <summary>
        /// 获取缓存的时间戳
        /// </summary>
        TimeSpan Expiration { get; }

        /// <summary>
        /// 返回请求对应的缓存的键
        /// 该键用于读取或写入缓存到缓存提供者
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        Task<string> GetCacheKeyAsync(ApiActionContext context);
    }
}
