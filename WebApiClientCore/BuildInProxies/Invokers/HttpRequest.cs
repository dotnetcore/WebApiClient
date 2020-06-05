using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;
using WebApiClientCore.ResponseCaches;

namespace WebApiClientCore
{
    /// <summary>
    /// 提供http请求
    /// </summary>
    static class HttpRequest
    {
        /// <summary>
        /// 发送http请求
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="ApiInvalidConfigException"></exception>
        /// <returns></returns>
        public static async Task<ApiResponseContext> SendAsync(ApiRequestContext context)
        {
            if (context.HttpContext.RequestMessage.RequestUri == null)
            {
                throw new ApiInvalidConfigException(Resx.required_HttpHost);
            }

            var actionCache = await GetCaheAsync(context).ConfigureAwait(false);
            if (actionCache != null && actionCache.Value != null)
            {
                context.HttpContext.ResponseMessage = actionCache.Value;
            }
            else
            {
                var client = context.HttpContext.HttpClient;
                var request = context.HttpContext.RequestMessage;
                using var tokenLinker = new CancellationTokenLinker(context.HttpContext.CancellationTokens);
                var response = await client.SendAsync(request, tokenLinker.Token).ConfigureAwait(false);

                context.HttpContext.ResponseMessage = response;
                await SetCacheAsync(context, actionCache?.Key, response).ConfigureAwait(false);
            }
            return new ApiResponseContext(context);
        }

        /// <summary>
        /// 获取响应的缓存
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static async Task<ActionCache?> GetCaheAsync(ApiRequestContext context)
        {
            var attribute = context.ApiAction.CacheAttribute;
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
        private static async Task SetCacheAsync(ApiRequestContext context, string? cacheKey, HttpResponseMessage? response)
        {
            var attribute = context.ApiAction.CacheAttribute;
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

        /// <summary>
        /// 表示CancellationToken链接器
        /// </summary>
        private struct CancellationTokenLinker : IDisposable
        {
            /// <summary>
            /// 链接产生的tokenSource
            /// </summary>
            private readonly CancellationTokenSource? tokenSource;

            /// <summary>
            /// 获取token
            /// </summary>
            public CancellationToken Token { get; }

            /// <summary>
            /// CancellationToken链接器
            /// </summary>
            /// <param name="tokenList"></param>
            public CancellationTokenLinker(IList<CancellationToken> tokenList)
            {
                if (IsNoneCancellationToken(tokenList) == true)
                {
                    this.tokenSource = null;
                    this.Token = CancellationToken.None;
                }
                else
                {
                    this.tokenSource = CancellationTokenSource.CreateLinkedTokenSource(tokenList.ToArray());
                    this.Token = this.tokenSource.Token;
                }
            }

            /// <summary>
            /// 是否为None的CancellationToken
            /// </summary>
            /// <param name="tokenList"></param>
            /// <returns></returns>
            private static bool IsNoneCancellationToken(IList<CancellationToken> tokenList)
            {
                if (tokenList.Count == 0)
                {
                    return true;
                }
                if (tokenList.Count == 1 && tokenList[0] == CancellationToken.None)
                {
                    return true;
                }
                return false;
            }

            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                if (this.tokenSource != null)
                {
                    this.tokenSource.Dispose();
                }
            }
        }
    }
}
