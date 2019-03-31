#if !NETSTANDARD1_3
using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示Api响应结果缓存提供者的接口
    /// </summary>
    class ResponseCacheProvider : IResponseCacheProvider
    {
        /// <summary>
        /// 默认实例
        /// </summary>
        public static readonly ResponseCacheProvider Instance = new ResponseCacheProvider();

        /// <summary>
        /// 内存缓存
        /// </summary>
        private readonly MemoryCache cache = new MemoryCache(Guid16.NewGuid16().ToString());

        /// <summary>
        /// 获取提供者的友好名称
        /// </summary>
        public string Name { get; } = nameof(MemoryCache);

        /// <summary>
        /// 从缓存中获取响应实体
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public Task<ResponseCacheResult> GetAsync(string key)
        {
            var cacheItem = this.cache.GetCacheItem(key);
            if (cacheItem == null)
            {
                var result = new ResponseCacheResult(null, false);
                return Task.FromResult(result);
            }
            else
            {
                var result = new ResponseCacheResult((ResponseCacheEntry)cacheItem.Value, true);
                return Task.FromResult(result);
            }
        }

        /// <summary>
        /// 设置响应实体到缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="entry">缓存实体</param>
        /// <param name="expiration">有效时间</param>
        /// <returns></returns>
        public Task SetAsync(string key, ResponseCacheEntry entry, TimeSpan expiration)
        {
            this.cache.Set(key, entry, new DateTimeOffset(DateTime.Now.Add(expiration)));
            return ApiTask.CompletedTask;
        }
    }
}
#endif