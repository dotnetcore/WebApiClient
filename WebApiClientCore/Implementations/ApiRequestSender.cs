using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 提供 http 请求
    /// </summary>
    static class ApiRequestSender
    {
        /// <summary>
        /// 发送 http 请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requestAborted"></param>
        /// <exception cref="ApiInvalidConfigException"></exception>
        /// <returns></returns>
        public static async Task<ApiResponseContext> SendAsync(ApiRequestContext context, CancellationToken requestAborted)
        {
            if (context.HttpContext.RequestMessage.RequestUri == null)
            {
                throw new ApiInvalidConfigException(Resx.required_HttpHost);
            }

            try
            {
                await SendCoreAsync(context, requestAborted).ConfigureAwait(false);
                return new ApiResponseContext(context, requestAborted);
            }
            catch (Exception ex)
            {
                return new ApiResponseContext(context, requestAborted) { Exception = ex };
            }
        }

        /// <summary>
        /// 发送 http 请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requestAborted"></param>
        /// <exception cref="HttpRequestException"></exception> 
        /// <returns></returns>
        private static async Task SendCoreAsync(ApiRequestContext context, CancellationToken requestAborted)
        {
            var actionCache = await context.GetCacheAsync().ConfigureAwait(false);
            if (actionCache != null && actionCache.Value != null)
            {
                context.HttpContext.ResponseMessage = actionCache.Value;
            }
            else
            {
                var client = context.HttpContext.HttpClient;
                var request = context.HttpContext.RequestMessage;
                var completionOption = context.GetCompletionOption();

                var response = await client.SendAsync(request, completionOption, requestAborted).ConfigureAwait(false);
                context.HttpContext.ResponseMessage = response;
                await context.SetCacheAsync(actionCache?.Key, response).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 获取响应的缓存
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static async Task<ActionCache?> GetCacheAsync(this ApiRequestContext context)
        {
            var attribute = context.ActionDescriptor.CacheAttribute;
            if (attribute == null)
            {
                return default;
            }

            if (attribute.GetReadPolicy(context) == CachePolicy.Ignore)
            {
                return default;
            }

            var cacheKey = await attribute.GetCacheKeyAsync(context).ConfigureAwait(false);
            if (string.IsNullOrEmpty(cacheKey))
            {
                return default;
            }

            var provider = attribute.GetCacheProvider(context);
            if (provider == null)
            {
                return default;
            }

            var responseCache = await provider.GetAsync(cacheKey).ConfigureAwait(false);
            if (responseCache.HasValue == false || responseCache.Value == null)
            {
                return new ActionCache(cacheKey, null);
            }

            var request = context.HttpContext.RequestMessage;
            var response = responseCache.Value.ToResponseMessage(request, provider.Name);
            return new ActionCache(cacheKey, response);
        }

        /// <summary>
        /// 更新响应到缓存
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cacheKey">缓存键</param>
        /// <param name="response">响应消息</param>
        /// <returns></returns>
        private static async Task SetCacheAsync(this ApiRequestContext context, string? cacheKey, HttpResponseMessage? response)
        {
            var attribute = context.ActionDescriptor.CacheAttribute;
            if (attribute == null)
            {
                return;
            }

            if (response == null)
            {
                return;
            }

            if (attribute.GetWritePolicy(context) == CachePolicy.Ignore)
            {
                return;
            }

            if (string.IsNullOrEmpty(cacheKey) == true)
            {
                cacheKey = await attribute.GetCacheKeyAsync(context).ConfigureAwait(false);
            }

            if (cacheKey == null)
            {
                return;
            }

            var provider = attribute.GetCacheProvider(context);
            if (provider == null)
            {
                return;
            }

            var cacheEntry = await ResponseCacheEntry.FromResponseMessageAsync(response).ConfigureAwait(false);
            await provider.SetAsync(cacheKey, cacheEntry, attribute.Expiration).ConfigureAwait(false);
        }


        /// <summary>
        /// 表示Action缓存结果
        /// </summary>
        private class ActionCache
        {
            /// <summary>
            /// 获取缓存的键
            /// </summary>
            public string Key { get; }

            /// <summary>
            /// 获取响应信息
            /// </summary>
            public HttpResponseMessage? Value { get; set; }

            /// <summary>
            /// 缓存结果
            /// </summary>
            /// <param name="key">缓存的键</param>
            /// <param name="value">响应信息</param>
            public ActionCache(string key, HttpResponseMessage? value)
            {
                this.Key = key;
                this.Value = value;
            }
        }
    }
}
