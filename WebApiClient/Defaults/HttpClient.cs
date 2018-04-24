using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClient.Defaults
{
    /// <summary>
    /// 表示默认的HttpClient
    /// </summary>
    public class HttpClient : IHttpClient
    {
        /// <summary>
        /// 是否已释放
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// 正在挂起的请求
        /// </summary>
        private long pendingCount = 0L;

        /// <summary>
        /// HttpClient实例
        /// </summary>
        private System.Net.Http.HttpClient httpClient;

        /// <summary>
        /// 是否支持创建Handler
        /// </summary>
        private readonly bool supportCreateHandler = false;


        /// <summary>
        /// 获取关联的Http处理对象的IHttpHandler包装
        /// </summary>
        public IHttpHandler Handler { get; private set; }


        /// <summary>
        /// 获取默认的请求头管理对象
        /// </summary>
        public HttpRequestHeaders DefaultRequestHeaders
        {
            get => this.httpClient.DefaultRequestHeaders;
        }

        /// <summary>
        /// 获取或设置请求超时时间
        /// </summary>
        public TimeSpan Timeout
        {
            get => this.httpClient.Timeout;
            set => this.httpClient.Timeout = value;
        }

        /// <summary>
        /// 获取或设置最大回复内容长度
        /// </summary>
        public long MaxResponseContentBufferSize
        {
            get => this.httpClient.MaxResponseContentBufferSize;
            set => this.httpClient.MaxResponseContentBufferSize = value;
        }

        /// <summary>
        /// 默认的HttpClient
        /// </summary>
        public HttpClient() :
            this(handler: null, disposeHandler: true, supportCreateHandler: true)
        {
        }

        /// <summary>
        /// 默认的HttpClient
        /// </summary>
        /// <param name="handler">关联的Http处理对象</param>
        /// <param name="disposeHandler">调用Dispose方法时，是否也Dispose handler</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public HttpClient(HttpMessageHandler handler, bool disposeHandler = false)
            : this(handler ?? throw new ArgumentNullException(nameof(handler)), disposeHandler, false)
        {
        }

        /// <summary>
        /// 默认的HttpClient
        /// </summary>
        /// <param name="handler"></param>   
        /// <param name="disposeHandler">调用HttpClient.Dispose时是否也disposeHandler</param>
        /// <param name="supportCreateHandler">是否支持调用创建实例</param>
        /// <exception cref="ArgumentException"></exception>
        private HttpClient(HttpMessageHandler handler, bool disposeHandler, bool supportCreateHandler)
        {
            this.supportCreateHandler = supportCreateHandler;
            this.Handler = handler == null ? this.CreateIHandler() : HttpHandler.From(handler);
            this.httpClient = new System.Net.Http.HttpClient(this.Handler.InnerHanlder, disposeHandler);
        }

        /// <summary>
        /// 设置Cookie值到Cookie容器
        /// 当Handler.UseCookies才添加
        /// </summary>
        /// <param name="domain">cookie域名</param>
        /// <param name="cookieValues">cookie值，会自动进行URL编码，eg：key1=value1; key2=value2</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="CookieException"></exception>
        /// <returns></returns>
        public bool SetCookie(Uri domain, string cookieValues)
        {
            return this.SetCookie(domain, cookieValues, true);
        }

        /// <summary>
        /// 设置Cookie值到Cookie容器
        /// 当Handler.UseCookies才添加
        /// </summary>
        /// <param name="domain">cookie域名</param>
        /// <param name="cookieValues">cookie值，不进行URL编码，eg：key1=value1; key2=value2</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="CookieException"></exception>
        /// <returns></returns>
        public bool SetRawCookie(Uri domain, string cookieValues)
        {
            return this.SetCookie(domain, cookieValues, false);
        }

        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="domain">cookie域名</param>
        /// <param name="cookieValues">cookie值</param>
        /// <param name="useUrlEncode">是否URL编码</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="CookieException"></exception>
        /// <returns></returns>
        private bool SetCookie(Uri domain, string cookieValues, bool useUrlEncode)
        {
            if (this.Handler.UseCookies == false)
            {
                return false;
            }

            if (domain == null)
            {
                throw new ArgumentNullException(nameof(domain));
            }

            foreach (var cookie in HttpUtility.ParseCookie(cookieValues, useUrlEncode))
            {
                this.Handler.CookieContainer.Add(domain, cookie);
            }
            return true;
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

            if (this.Handler.SupportsProxy == false)
            {
                return false;
            }

            if (HttpProxy.IsProxyEquals(this.Handler.Proxy, proxy) == true)
            {
                return false;
            }

            // 设置代理前释放实例并重新初始化
            if (this.Handler.Proxy != null)
            {
                this.InitWithoutProxy();
            }

            this.Handler.UseProxy = proxy != null;
            this.Handler.Proxy = proxy;
            return true;
        }

        /// <summary>
        /// 重新初始化HttpClient和Handler实例
        /// </summary>
        private void InitWithoutProxy()
        {
            var newHandler = this.CreateIHandler();
            Property.CopyProperties(this.Handler.InnerHanlder, newHandler.InnerHanlder);
            newHandler.UseProxy = false;
            newHandler.Proxy = null;

            var newClient = new System.Net.Http.HttpClient(newHandler.InnerHanlder);
            Property.CopyProperties(this.httpClient, newClient);

            this.httpClient.Dispose();
            this.httpClient = newClient;
            this.Handler = newHandler;
        }

        /// <summary>
        /// 创建IHandler的新实例
        /// </summary>
        /// <returns></returns>
        protected virtual IHttpHandler CreateIHandler()
        {
            if (this.supportCreateHandler == false)
            {
                throw new NotSupportedException("不支持创建新的HttpClientHandler实例");
            }
            return HttpHandler.CreateHanlder();
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
                throw new HttpApiConfigException("未配置RequestUri，RequestUri不能为null");
            }

            try
            {
                Interlocked.Increment(ref this.pendingCount);

                var timeout = request.Timeout ?? this.Timeout;
                var cancellationToken = new CancellationTokenSource(timeout).Token;
                return await this.httpClient.SendAsync(request, cancellationToken);
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
            this.httpClient.CancelPendingRequests();
        }

        /// <summary>
        /// 释放httpClient
        /// </summary>
        public void Dispose()
        {
            this.httpClient.Dispose();
            this.isDisposed = true;
        }
    }
}
