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
        private readonly bool enable;
        private readonly ApiActionContext context;
        private readonly IResponseCacheProvider provider;
        private readonly IApiCacheAttribute attribute;

        /// <summary>
        /// Api缓存
        /// </summary>
        /// <param name="context">上下文</param>
        public ApiCache(ApiActionContext context)
        {
            this.context = context;
            this.attribute = context.ApiAction.CacheAttribute;
            this.provider = context.HttpContext.Services.GetService<IResponseCacheProvider>();
            this.enable = this.attribute != null && this.provider != null;
        }

        /// <summary>
        /// 获取响应的缓存
        /// </summary>
        /// <returns></returns>
        public async Task<CacheResult> GetAsync()
        {
            if (this.enable == false)
            {
                return CacheResult.Empty;
            }

            if (this.attribute.GetReadPolicy(this.context) == CachePolicy.Ignore)
            {
                return CacheResult.Empty;
            }

            var cacheKey = await this.attribute.GetCacheKeyAsync(this.context).ConfigureAwait(false);
            if (string.IsNullOrEmpty(cacheKey) == true)
            {
                return CacheResult.Empty;
            }

            var cacheResult = await this.provider.GetAsync(cacheKey).ConfigureAwait(false);
            if (cacheResult.HasValue == false || cacheResult.Value == null)
            {
                return new CacheResult(cacheKey, null);
            }

            var response = cacheResult.Value.ToResponseMessage(this.context.HttpContext.RequestMessage, this.provider.Name);
            return new CacheResult(cacheKey, response);
        }

        /// <summary>
        /// 更新响应到缓存
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <returns></returns>
        public async Task SetAsync(string cacheKey)
        {
            if (this.enable == false)
            {
                return;
            }

            if (this.context.HttpContext.ResponseMessage == null)
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

            if (string.IsNullOrEmpty(cacheKey) == true)
            {
                return;
            }

            var httpResponse = this.context.HttpContext.ResponseMessage;
            var cacheEntry = await ResponseCacheEntry.FromResponseMessageAsync(httpResponse).ConfigureAwait(false);
            await this.provider.SetAsync(cacheKey, cacheEntry, attribute.Expiration).ConfigureAwait(false);
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
