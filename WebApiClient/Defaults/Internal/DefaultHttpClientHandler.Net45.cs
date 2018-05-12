#if NET45

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClient.Defaults
{
    /// <summary>
    /// 默认的HttpClientHandler
    /// </summary>
    class DefaultHttpClientHandler : WebRequestHandler, IHttpHandler
    {
        /// <summary>
        /// 发送次数
        /// </summary>
        private int sendTimes = 0;

        /// <summary>
        /// 同步锁
        /// </summary>
        private readonly object syncRoot = new object();

        /// <summary>
        /// 站点地址
        /// </summary>
        private readonly HashSet<Uri> hashSet = new HashSet<Uri>(new UriComparer());

        /// <summary>
        /// 每个服务的最大连接数设置器
        /// </summary>
        private static readonly PropertySetter maxConnectionsPerServerSetter;

        /// <summary>
        /// 获取内部的原始Handler对象
        /// </summary>
        public HttpMessageHandler InnerHanlder
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// 静态构造器
        /// </summary>
        static DefaultHttpClientHandler()
        {
            var property = typeof(HttpClientHandler).GetProperty("MaxConnectionsPerServer", typeof(int));
            if (property != null)
            {
                maxConnectionsPerServerSetter = new PropertySetter(property);
            }
        }

        /// <summary>
        /// HttpClientHandler
        /// </summary>
        public DefaultHttpClientHandler()
        {
            this.UseProxy = false;
            this.Proxy = null;
            this.ServerCertificateValidationCallback = (a, b, c, d) => true;

            if (maxConnectionsPerServerSetter != null)
            {
                maxConnectionsPerServerSetter.Invoke(this, HttpApiClient.ConnectionLimit);
            }
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // 通过ServicePoint设置最大连接数
            if (maxConnectionsPerServerSetter == null)
            {
                this.SetServicePointConnectionLimit(request.RequestUri, HttpApiClient.ConnectionLimit);
            }

            var header = request.Headers;
            var isClose = header.ConnectionClose == true || header.Connection.Contains("close");

            header.Connection.Clear();
            header.ConnectionClose = isClose;

            var isFirstSend = Interlocked.CompareExchange(ref this.sendTimes, 1, 0) == 0;
            if (isClose == false && isFirstSend == false)
            {
                header.Connection.Add("keep-alive");
            }
            return base.SendAsync(request, cancellationToken);
        }

        /// <summary>
        /// 通过ServicePoint设置最大连接数
        /// 如果其它实例也操作ServicePoint，将影响到本实例
        /// 每个站点只设置一次
        /// </summary>
        /// <param name="address">站点地址</param>
        /// <param name="limit">最大连接数</param>
        private void SetServicePointConnectionLimit(Uri address, int limit)
        {
            if (this.Proxy != null)
            {
                address = this.Proxy.GetProxy(address);
            }

            lock (this.syncRoot)
            {
                if (this.hashSet.Add(address) == true)
                {
                    var point = ServicePointManager.FindServicePoint(address);
                    point.ConnectionLimit = limit;
                }
            }
        }

        /// <summary>
        /// Uri比较器
        /// </summary>
        private class UriComparer : IEqualityComparer<Uri>
        {
            public bool Equals(Uri x, Uri y)
            {
                return true;
            }

            public int GetHashCode(Uri obj)
            {
                return obj.Authority.GetHashCode();
            }
        }
    }
}

#endif