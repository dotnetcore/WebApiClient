using System;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 定义Api请求结果缓存提供者的接口
    /// </summary>
    public interface IApiCacheProvider
    {
        /// <summary>
        /// 获取缓存结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey">键</param>
        /// <returns></returns>
        Task<CacheResult<T>> GetAsync<T>(string cacheKey) where T : class;

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey">键</param>
        /// <param name="value">缓存值</param>
        /// <param name="expiration">有效时间</param>
        /// <returns></returns>
        Task SetAsync<T>(string cacheKey, T value, TimeSpan expiration) where T : class;
    }
}
