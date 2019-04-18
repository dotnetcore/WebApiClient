using System.Net.Http;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 表示Api缓存
    /// </summary>
    class ApiCache
    {
        private readonly bool useCache;
        private readonly ApiActionContext context;
        private readonly IResponseCacheProvider cacheProvider;
        private readonly IApiActionCacheAttribute cacheAttribute;

        /// <summary>
        /// Api缓存
        /// </summary>
        /// <param name="context">上下文</param>
        public ApiCache(ApiActionContext context)
        {
            this.context = context;
            this.cacheAttribute = context.ApiActionDescriptor.Cache;
            this.cacheProvider = context.HttpApiConfig.ResponseCacheProvider;
            this.useCache = this.cacheAttribute != null && this.cacheProvider != null;
        }

        /// <summary>
        /// 获取响应的缓存
        /// </summary>
        /// <returns></returns>
        public async Task<CacheResult> GetAsync()
        {
            if (this.useCache == false)
            {
                return CacheResult.Empty;
            }

            var willRead = true;
            if (this.cacheAttribute is IApiActionCachePolicyAttribute cachePolicy)
            {
                willRead = cachePolicy.NeedReadCache(this.context);
            }

            if (willRead == false)
            {
                return CacheResult.Empty;
            }

            var cacheKey = await this.cacheAttribute.GetCacheKeyAsync(this.context).ConfigureAwait(false);
            if (string.IsNullOrEmpty(cacheKey) == true)
            {
                return CacheResult.Empty;
            }

            var cacheResult = await this.cacheProvider.GetAsync(cacheKey).ConfigureAwait(false);
            if (cacheResult.HasValue == false || cacheResult.Value == null)
            {
                return new CacheResult(cacheKey, null);
            }

            var response = cacheResult.Value.ToResponseMessage(this.context.RequestMessage, cacheProvider.Name);
            return new CacheResult(cacheKey, response);
        }

        /// <summary>
        /// 更新响应到缓存
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <returns></returns>
        public async Task SetAsync(string cacheKey)
        {
            if (this.useCache == false)
            {
                return;
            }

            var willWrite = true;
            if (this.cacheAttribute is IApiActionCachePolicyAttribute cachePolicy)
            {
                willWrite = cachePolicy.NeedWriteCache(this.context);
            }

            if (willWrite == false)
            {
                return;
            }

            if (string.IsNullOrEmpty(cacheKey) == true)
            {
                cacheKey = await this.cacheAttribute.GetCacheKeyAsync(this.context).ConfigureAwait(false);
                if (string.IsNullOrEmpty(cacheKey) == true)
                {
                    return;
                }
            }

            var httpResponse = this.context.ResponseMessage;
            var cacheEntry = await ResponseCacheEntry.FromResponseMessageAsync(httpResponse).ConfigureAwait(false);
            await cacheProvider.SetAsync(cacheKey, cacheEntry, cacheAttribute.Expiration).ConfigureAwait(false);
        }

        /// <summary>
        /// 表示缓存结果
        /// </summary>
        public struct CacheResult
        {
            /// <summary>
            /// 获取缓存的键
            /// </summary>
            public string CacheKey { get; }

            /// <summary>
            /// 获取响应信息
            /// </summary>
            public HttpResponseMessage ResponseMessage { get; }

            /// <summary>
            /// 获取空的缓存结果
            /// </summary>
            public static CacheResult Empty { get; } = new CacheResult(null, null);

            /// <summary>
            /// 缓存结果
            /// </summary>
            /// <param name="cacheKey">缓存的键</param>
            /// <param name="responseMessage">响应信息</param>
            public CacheResult(string cacheKey, HttpResponseMessage responseMessage)
            {
                this.CacheKey = cacheKey;
                this.ResponseMessage = responseMessage;
            }
        }
    }
}
