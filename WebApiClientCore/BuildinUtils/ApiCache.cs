using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示Api缓存
    /// </summary>
    class ApiCache
    {
        private readonly ApiRequestContext context;
        private readonly IApiCacheAttribute attribute;

        /// <summary>
        /// Api缓存
        /// </summary>
        /// <param name="context">上下文</param>
        public ApiCache(ApiRequestContext context)
        {
            this.context = context;
            this.attribute = context.ApiAction.CacheAttribute;
        }

        /// <summary>
        /// 获取响应的缓存
        /// </summary>
        /// <returns></returns>
        public async Task<ApiCacheValue?> GetAsync()
        {
            if (this.attribute == null)
            {
                return default;
            }

            if (this.attribute.GetReadPolicy(this.context) == CachePolicy.Ignore)
            {
                return default;
            }

            var cacheKey = await this.attribute.GetCacheKeyAsync(this.context).ConfigureAwait(false);
            if (string.IsNullOrEmpty(cacheKey) == true)
            {
                return default;
            }

            var provider = this.context.HttpContext.Services.GetService<IResponseCacheProvider>();
            if (provider == null)
            {
                return default;
            }

            var responseCache = await provider.GetAsync(cacheKey).ConfigureAwait(false);
            if (responseCache.HasValue == false || responseCache.Value == null)
            {
                return new ApiCacheValue(cacheKey, null);
            }

            var request = this.context.HttpContext.RequestMessage;
            var response = responseCache.Value.ToResponseMessage(request, provider.Name);
            return new ApiCacheValue(cacheKey, response);
        }

        /// <summary>
        /// 更新响应到缓存
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <param name="response">响应消息</param>
        /// <returns></returns>
        public async Task SetAsync(string? cacheKey, HttpResponseMessage? response)
        {
            if (this.attribute == null)
            {
                return;
            }

            if (response == null)
            {
                return;
            }

            if (this.attribute.GetWritePolicy(this.context) == CachePolicy.Ignore)
            {
                return;
            }

            if (string.IsNullOrEmpty(cacheKey) == true)
            {
                cacheKey = await this.attribute.GetCacheKeyAsync(this.context).ConfigureAwait(false);
            }

            if (cacheKey == null)
            {
                return;
            }

            var provider = this.context.HttpContext.Services.GetService<IResponseCacheProvider>();
            if (provider == null)
            {
                return;
            }

            var cacheEntry = await ResponseCacheEntry.FromResponseMessageAsync(response).ConfigureAwait(false);
            await provider.SetAsync(cacheKey, cacheEntry, attribute.Expiration).ConfigureAwait(false);
        }
    }
}
