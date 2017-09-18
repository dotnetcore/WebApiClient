using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// HttpClientContext提供者
    /// </summary>
    class DefaultHttpClientContextProvider : IHttpClientContextProvider, IDisposable
    {
        /// <summary>
        /// 获取HttpClient处理者
        /// </summary>
        private readonly HttpClientContext httpClientContext = new HttpClientContext();

        /// <summary>
        /// 在请求前将创建IHttpClientContext
        /// </summary>
        /// <param name="context">请求上下文</param>
        /// <returns></returns>
        public IHttpClientContext CreateHttpClientContext(ApiActionContext context)
        {
            return this.httpClientContext;
        }

        /// <summary>
        /// <summary>
        /// 在Http请求完成之后释放HttpClientContext 
        /// </summary>
        /// <param name="context">HttpClient上下文</param>
        public void DisponseHttpClientContext(IHttpClientContext context)
        {
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            this.httpClientContext.Dispose();
        }

        /// <summary>
        /// 表示HttpClient上下文
        /// </summary>
        class HttpClientContext : IHttpClientContext, IDisposable
        {
            /// <summary>
            /// 获取HttpClient实例
            /// </summary>
            public HttpClient HttpClient { get; private set; }

            /// <summary>
            /// 获取HttpClient处理者
            /// </summary>
            public HttpClientHandler HttpClientHandler { get; private set; }

            /// <summary>
            /// HttpClient上下文
            /// </summary>
            public HttpClientContext()
            {
                this.HttpClientHandler = new KeepAliveHandler(keepAlive: true)
                {
                    // 默认开户Gzip请求
                    AutomaticDecompression = DecompressionMethods.GZip,
                };

                this.HttpClient = new HttpClient(this.HttpClientHandler, false);
            }

            /// <summary>
            /// 释放
            /// </summary>
            public void Dispose()
            {
                this.HttpClient.Dispose();
                this.HttpClientHandler.Dispose();
            }
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
                if (this.keepAlive == true)
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
