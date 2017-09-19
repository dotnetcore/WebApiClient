using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 默认的HttpClientHandler
    /// </summary>
    class DefaultHttpClientHandler : HttpClientHandler
    {
        /// <summary>
        /// 发送次数
        /// </summary>
        private int sendTimes = 0;

        /// <summary>
        /// 获取是否keepAlive
        /// </summary>
        public bool KeepAlive { get; private set; }

        /// <summary>
        /// keep-alive的HttpClientHandler
        /// </summary>
        /// <param name="keepAlive">keepAlive</param>
        public DefaultHttpClientHandler(bool keepAlive)
        {
            this.KeepAlive = keepAlive;
            this.UseProxy = false;
            this.AutomaticDecompression = DecompressionMethods.GZip;
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Remove("Connection");

            if (this.KeepAlive == false)
            {
                request.Headers.Add("Connection", "close");
            }
            else
            {
                if (Interlocked.CompareExchange(ref this.sendTimes, 1, 0) == 0)
                {
                    request.Headers.Add("Connection", string.Empty);
                }
                else
                {
                    request.Headers.Add("Connection", "keep-alive");
                }
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}
