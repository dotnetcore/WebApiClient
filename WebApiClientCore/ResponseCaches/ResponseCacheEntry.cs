using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApiClientCore.ResponseCaches
{
    /// <summary>
    /// 表示要缓存的响应实体
    /// </summary>
    public class ResponseCacheEntry
    {
        /// <summary>
        /// 获取或设置版本号
        /// </summary>
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置状态码
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// 获取或设置状态说明
        /// </summary>
        public string? ReasonPhrase { get; set; }

        /// <summary>
        /// 获取或设置请求头
        /// </summary>
        public Dictionary<string, string[]>? Headers { get; set; }

        /// <summary>
        /// 获取或设置内容的请求头
        /// </summary>
        public Dictionary<string, string[]>? ContentHeaders { get; set; }

        /// <summary>
        /// 获取或设置响应内容
        /// </summary>
        public byte[] Content { get; set; } = Array.Empty<byte>();


        /// <summary>
        /// 转换为HttpResponseMessage
        /// </summary>
        /// <param name="requestMessage">请求信息</param>
        /// <param name="cacheProviderName">缓存提供者名</param>
        /// <returns></returns>
        public HttpResponseMessage ToResponseMessage(HttpRequestMessage requestMessage, string cacheProviderName)
        {
            var response = new HttpResponseMessage
            {
                Content = new ByteArrayContent(this.Content),
                ReasonPhrase = this.ReasonPhrase,
                RequestMessage = requestMessage,
                StatusCode = this.StatusCode,
                Version = System.Version.Parse(this.Version)
            };

            if (this.Headers != null)
            {
                foreach (var item in this.Headers)
                {
                    response.Headers.TryAddWithoutValidation(item.Key, item.Value);
                }
            }
            if (this.ContentHeaders != null)
            {
                foreach (var item in this.ContentHeaders)
                {
                    response.Content.Headers.TryAddWithoutValidation(item.Key, item.Value);
                }
            }
            if (string.IsNullOrEmpty(cacheProviderName) == false)
            {
                response.Headers.TryAddWithoutValidation("Response-Cache-Provider", cacheProviderName);
            }
            return response;
        }

        /// <summary>
        /// 从HttpResponseMessage转换得到
        /// </summary>
        /// <param name="responseMessage">响应消息</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static async Task<ResponseCacheEntry> FromResponseMessageAsync(HttpResponseMessage responseMessage)
        {
            if (responseMessage == null)
            {
                throw new ArgumentNullException(nameof(responseMessage));
            }

            return new ResponseCacheEntry
            {
                Content = await responseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false),
                ContentHeaders = responseMessage.Content.Headers.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray()),
                Headers = responseMessage.Headers.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray()),
                ReasonPhrase = responseMessage.ReasonPhrase,
                StatusCode = responseMessage.StatusCode,
                Version = responseMessage.Version.ToString()
            };
        }
    }
}
