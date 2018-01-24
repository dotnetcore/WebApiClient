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
using WebApiClient.Interfaces;

namespace WebApiClient.Defaults
{
    /// <summary>
    /// 表示默认的HttpClient
    /// </summary>
    public class HttpClient : IHttpClient
    {
        /// <summary>
        /// HttpClient实例
        /// </summary>
        private System.Net.Http.HttpClient client;


        /// <summary>
        /// 是否已释放
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// 正在挂起的请求
        /// </summary>
        private long pendingCount = 0L;

        /// <summary>
        /// 是否支持创建Handler
        /// </summary>
        private bool supportCreateHandler = false;

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
        public HttpClient(HttpClientHandler handler, bool disposeHandler = false)
            : this(handler, disposeHandler, false)
        {
        }

        /// <summary>
        /// 默认的HttpClient
        /// </summary>
        /// <param name="handler"></param>   
        /// <param name="disposeHandler">调用HttpClient.Dispose时是否也disposeHandler</param>
        /// <param name="supportCreateHandler">是否支持调用创建实例</param>
        /// <exception cref="ArgumentNullException"></exception>
        private HttpClient(HttpClientHandler handler, bool disposeHandler, bool supportCreateHandler)
        {
            this.supportCreateHandler = supportCreateHandler;
            if (handler == null)
            {
                if (supportCreateHandler == false)
                {
                    throw new ArgumentNullException(nameof(handler));
                }
                else
                {
                    handler = this.CreateHttpClientHandler();
                }
            }

            this.Handler = handler;
            this.client = new System.Net.Http.HttpClient(this.Handler, disposeHandler);

#if NETCOREAPP2_0
            this.Handler.MaxConnectionsPerServer = HttpApiClient.ConnectionLimit;
#else
            MaxConnectionsPerServer.Set(this.Handler, HttpApiClient.ConnectionLimit);
#endif
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

            if (this.Handler.SupportsProxy == false)
            {
                return false;
            }

            if (this.IsProxyEquals(this.Handler.Proxy, proxy) == true)
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
            var handler = this.CreateHttpClientHandler();
            this.CopyProperties(this.Handler, handler);
            handler.UseProxy = false;
            handler.Proxy = null;

            var httpClient = new System.Net.Http.HttpClient(handler);
            this.CopyProperties(this.client, httpClient);
            this.client.Dispose();

            this.client = httpClient;
            this.Handler = handler;
        }

        /// <summary>
        /// 创建HttpClientHandler的新实例
        /// </summary>
        /// <returns></returns>
        protected virtual HttpClientHandler CreateHttpClientHandler()
        {
            if (this.supportCreateHandler == false)
            {
                throw new NotSupportedException("不支持创建新的HttpClientHandler实例");
            }
            return new DefaultHttpClientHandler();
        }

        /// <summary>
        /// 复制source的属性到target
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private bool CopyProperties<T>(T source, T target)
        {
            var state = true;
            var properties = source.GetType()
                .GetProperties()
                .Where(item => item.CanRead && item.CanWrite);

            foreach (var propery in properties)
            {
                try
                {
                    var value = propery.GetValue(source);
                    propery.SetValue(target, value);
                }
                catch (Exception)
                {
                    state = false;
                }
            }
            return state;
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
                throw new HttpApiConfigException("未配置RequestUri，RequestUri不能为null");
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
        /// 最多连接数
        /// </summary>
        private static class MaxConnectionsPerServer
        {
            private static readonly PropertyGetter getter;
            private static readonly PropertySetter setter;

            /// <summary>
            /// 静态构造器
            /// </summary>
            static MaxConnectionsPerServer()
            {
                var property = typeof(HttpClientHandler).GetProperty("MaxConnectionsPerServer", typeof(int));
                if (property != null)
                {
                    getter = new PropertyGetter(property);
                    setter = new PropertySetter(property);
                }
            }

            /// <summary>
            /// 获取MaxConnectionsPerServer
            /// </summary>
            /// <param name="handler"></param>
            /// <returns></returns>
            public static int Get(HttpClientHandler handler)
            {
                if (getter == null)
                {
                    return ServicePointManager.DefaultConnectionLimit;
                }
                else
                {
                    return (int)getter.Invoke(handler);
                }
            }

            /// <summary>
            /// 设置MaxConnectionsPerServer
            /// </summary>
            /// <param name="handler"></param>
            /// <param name="value">最多连接数</param>
            public static void Set(HttpClientHandler handler, int value)
            {
                if (setter == null)
                {
                    ServicePointManager.DefaultConnectionLimit = value;
                }
                else
                {
                    setter.Invoke(handler, value);
                }
            }
        }

        /// <summary>
        /// 默认的HttpClientHandler
        /// </summary>
        private class DefaultHttpClientHandler : HttpClientHandler
        {
#if NET45
            /// <summary>
            /// 发送次数
            /// </summary>
            private int sendTimes = 0;
#endif

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
                this.Proxy = null;
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
#if NET45
                else if (Interlocked.CompareExchange(ref this.sendTimes, 1, 0) == 1)
#else
                else
#endif
                {
                    request.Headers.Connection.Add("keep-alive");
                }
                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}