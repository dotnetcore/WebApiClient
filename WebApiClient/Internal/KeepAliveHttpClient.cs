using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 修复keep-alive问题的HttpClient
    /// </summary>
    class KeepAliveHttpClient : HttpClient
    {
        /// <summary>
        /// 关联的HttpClientHandler
        /// </summary>
        public HttpClientHandler MessageHandler { get; private set; }

        /// <summary>
        /// keep-aliveHttpClient
        /// </summary>
        /// <param name="keepAlive"></param>
        public KeepAliveHttpClient(bool keepAlive)
            : this(new KeepAliveHandler(keepAlive))
        {
        }

        /// <summary>
        /// keep-aliveHttpClient
        /// </summary>
        /// <param name="handler">HttpMessageHandler</param>
        private KeepAliveHttpClient(KeepAliveHandler handler)
            : base(handler, true)
        {
            this.MessageHandler = handler;
        }

        /// <summary>
        /// 修复keep-alive问题的HttpClientHandler
        /// </summary>
        class KeepAliveHandler : HttpClientHandler
        {
            /// <summary>
            /// 发送次数
            /// </summary>
            private int sendTimes = 0;

            /// <summary>
            /// 是否keepAlive
            /// </summary>
            private readonly bool keepAlive;

            /// <summary>
            /// keep-alive的HttpClientHandler
            /// </summary>
            /// <param name="keepAlive">keepAlive</param>
            public KeepAliveHandler(bool keepAlive)
            {
                this.keepAlive = keepAlive;
            }

            /// <summary>
            /// 发送请求
            /// </summary>
            /// <param name="request"></param>
            /// <param name="cancellationToken"></param>
            /// <returns></returns>
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
            {
                request.Headers.Remove("Connection");

                if (this.keepAlive == false)
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
}
