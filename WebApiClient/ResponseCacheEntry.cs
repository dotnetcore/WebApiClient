using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// Represents the response entity to be cached
    /// </summary>
    public class ResponseCacheEntry
    {
        /// <summary>
        /// Gets or sets the version number
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the status code
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the status description
        /// </summary>
        public string ReasonPhrase { get; set; }

        /// <summary>
        /// Gets or sets the request header
        /// </summary>
        public Dictionary<string, string[]> Headers { get; set; }

        /// <summary>
        /// Get or set the content request header
        /// </summary>
        public Dictionary<string, string[]> ContentHeaders { get; set; }

        /// <summary>
        /// Get or set response content
        /// </summary>
        public byte[] Content { get; set; }


        /// <summary>
        /// Converted to HttpResponseMessage
        /// </summary>
        /// <param name="requestMessage">Request information</param>
        /// <param name="cacheProviderName">Cache provider name</param>
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
        /// Transformed from HttpResponseMessage
        /// </summary>
        /// <param name="responseMessage">Response message</param>
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
