using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClient
{

    /// <summary>
    /// 表示默认的HttpClient
    /// </summary>
    class DefaultHttpClient : IHttpClient
    {
        /// <summary>
        /// HttpClient实例
        /// </summary>
        private HttpClient client;

        /// <summary>
        /// 是否已释放
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// 正在挂起的请求
        /// </summary>
        private long pendingCount = 0L;

        /// <summary>
        /// 获取关联的Http处理对象
        /// </summary>
        public HttpClientHandler Handler { get; private set; }

        /// <summary>
        /// 获取默认的请求头管理对象
        /// </summary>
        public HttpRequestHeaders DefaultRequestHeaders
        {
            get
            {
                return this.client.DefaultRequestHeaders;
            }
        }

        /// <summary>
        /// 获取或设置请求超时时间
        /// </summary>
        public TimeSpan Timeout
        {
            get
            {
                return this.client.Timeout;
            }
            set
            {
                this.client.Timeout = value;
            }
        }

        /// <summary>
        /// 获取或设置最大回复内容长度
        /// </summary>
        public long MaxResponseContentBufferSize
        {
            get
            {
                return this.client.MaxResponseContentBufferSize;
            }
            set
            {
                this.client.MaxResponseContentBufferSize = value;
            }
        }

        /// <summary>
        /// 默认的HttpClient
        /// </summary>
        public DefaultHttpClient()
        {
            var handler = new DefaultHttpClientHandler();
            this.client = new HttpClient(handler);
            this.Handler = handler;
        }

        /// <summary>
        /// 可设置HttpMessageHandler
        /// </summary>
        public DefaultHttpClient(HttpMessageHandler handler)
        {
            this.client = new HttpClient(handler);
            this.Handler = (HttpClientHandler)handler;
        }

        /// <summary>
        /// 设置Cookie值到Cookie容器
        /// 当Handler.UseCookies才添加
        /// </summary>
        /// <param name="domain">cookie域名</param>
        /// <param name="cookieValues">cookie值，可以不编码，eg：key1=value1; key2=value2</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public bool SetCookie(Uri domain, string cookieValues)
        {
            if (this.Handler.UseCookies == false)
            {
                return false;
            }

            if (domain == null)
            {
                throw new ArgumentNullException(nameof(domain));
            }

            foreach (var cookie in this.EncodeCookies(cookieValues))
            {
                this.Handler.CookieContainer.Add(domain, cookie);
            }
            return true;
        }

        /// <summary>
        /// 给cookie编码
        /// </summary>
        /// <param name="cookieValues"></param>
        /// <returns></returns>
        private IEnumerable<Cookie> EncodeCookies(string cookieValues)
        {
            if (cookieValues == null)
            {
                return Enumerable.Empty<Cookie>();
            }

            return from item in cookieValues.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                   let kv = item.Split('=')
                   let name = kv.FirstOrDefault().Trim()
                   let value = kv.Length > 1 ? kv.LastOrDefault() : string.Empty
                   let encode = HttpUtility.UrlEncode(value, Encoding.UTF8)
                   select new Cookie(name, encode);
        }

        /// <summary>
        /// 设置代理
        /// </summary>
        /// <param name="proxy">代理，为null则清除代理</param>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public bool SetProxy(IWebProxy proxy)
        {
            if (this.isDisposed == true)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            if (Interlocked.Read(ref this.pendingCount) > 0L)
            {
                throw new InvalidOperationException("当前还有未完成的请求，不能更换代理");
            }

            if (this.IsProxyEquals(this.Handler.Proxy, proxy) == true)
            {
                return false;
            }

            // 设置代理前释放实例并重新初始化
            if (this.Handler.Proxy != null)
            {
                this.InitHttpClientWithoutProxy();
            }

            this.Handler.UseProxy = proxy != null;
            this.Handler.Proxy = proxy;
            return true;
        }

        /// <summary>
        /// 重新初始化HttpClient实例
        /// </summary>
        private void InitHttpClientWithoutProxy()
        {
            var withoutProxyHandler = ((DefaultHttpClientHandler)this.Handler).CloneWithoutProxy();
            var httpClient = new HttpClient(withoutProxyHandler)
            {
                Timeout = this.Timeout,
                MaxResponseContentBufferSize = this.MaxResponseContentBufferSize
            };

            foreach (var item in this.DefaultRequestHeaders)
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
            }

            this.client.Dispose();
            this.client = httpClient;
            this.Handler = withoutProxyHandler;
        }


        /// <summary>
        /// 比较代理是否相等
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool IsProxyEquals(IWebProxy x, IWebProxy y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            var destination = new Uri("http://www.webapiclient.com");
            return x.GetProxy(destination) == y.GetProxy(destination);
        }

        /// <summary>
        /// 异步发送请求
        /// </summary>
        /// <param name="request">请求消息</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> SendAsync(HttpApiRequestMessage request)
        {
            if (request.RequestUri == null)
            {
                throw new ApiConfigException("未配置RequestUri，RequestUri不能为null");
            }

            try
            {
                Interlocked.Increment(ref this.pendingCount);

                var timeout = request.Timeout.HasValue ? request.Timeout.Value : this.Timeout;
                var cancellationToken = new CancellationTokenSource(timeout).Token;
                return await this.client.SendAsync(request, cancellationToken);
            }
            finally
            {
                Interlocked.Decrement(ref this.pendingCount);
            }
        }

        /// <summary>
        /// 取消挂起的请求
        /// </summary>
        public void CancelPendingRequests()
        {
            this.client.CancelPendingRequests();
        }

        /// <summary>
        /// 释放httpClient
        /// </summary>
        public void Dispose()
        {
            this.client.Dispose();
            this.isDisposed = true;
        }



        /// <summary>
        /// 默认的HttpClientHandler
        /// </summary>
        private class DefaultHttpClientHandler : HttpClientHandler
        {
            /// <summary>
            /// 发送次数
            /// </summary>
            private int sendTimes = 0;

            /// <summary>
            /// 获取是否短连接
            /// </summary>
            public bool ConnectionClose { get; set; }

            /// <summary>
            /// HttpClientHandler
            /// </summary>
            public DefaultHttpClientHandler()
            {
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
                request.Headers.Connection.Clear();
                if (this.ConnectionClose == true)
                {
                    request.Headers.Connection.Add("close");
                }
                else if (Interlocked.CompareExchange(ref this.sendTimes, 1, 0) == 1)
                {
                    request.Headers.Connection.Add("keep-alive");
                }
                return base.SendAsync(request, cancellationToken);
            }

            /// <summary>
            /// 无代理克隆
            /// </summary>
            /// <returns></returns>
            public DefaultHttpClientHandler CloneWithoutProxy()
            {
                return new DefaultHttpClientHandler
                {
                    AllowAutoRedirect = this.AllowAutoRedirect,
                    AutomaticDecompression = this.AutomaticDecompression,
                    ClientCertificateOptions = this.ClientCertificateOptions,
                    ConnectionClose = this.ConnectionClose,
                    CookieContainer = this.CookieContainer,
                    Credentials = this.Credentials,
                    MaxAutomaticRedirections = this.MaxAutomaticRedirections,
                    MaxRequestContentBufferSize = this.MaxRequestContentBufferSize,
                    PreAuthenticate = this.PreAuthenticate,
                    UseProxy = false,
                    Proxy = null,
                    UseCookies = this.UseCookies,
                    UseDefaultCredentials = this.UseDefaultCredentials
                };
            }
        }
    }
}