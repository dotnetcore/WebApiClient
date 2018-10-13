#if NET45

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 默认的HttpClientHandler
    /// </summary>
    class DefaultHttpClientHandler : WebRequestHandler
    {
        /// <summary>
        /// Uri集合
        /// </summary>
        private readonly UriHashSet hashSet = new UriHashSet();

        /// <summary>
        /// 每个服务的最大连接数设置器
        /// </summary>
        private static readonly Action<HttpClientHandler, int> maxConnectionsPerServerAction;

        /// <summary>
        /// 静态构造器
        /// </summary>
        static DefaultHttpClientHandler()
        {
            var property = typeof(HttpClientHandler).GetProperty("MaxConnectionsPerServer", typeof(int));
            if (property != null)
            {
                maxConnectionsPerServerAction = Lambda.CreateSetAction<HttpClientHandler, int>(property);
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

            if (maxConnectionsPerServerAction != null)
            {
                maxConnectionsPerServerAction.Invoke(this, HttpApiClient.ConnectionLimit);
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
            if (maxConnectionsPerServerAction == null)
            {
                if (this.hashSet.Add(request.RequestUri) == true)
                {
                    var servicePoint = this.FindServicePoint(request.RequestUri);
                    servicePoint.ConnectionLimit = HttpApiClient.ConnectionLimit;
                }
            }

            return base.SendAsync(request, cancellationToken);
        }

        /// <summary>
        /// 查找Uri对应的ServicePoint
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private ServicePoint FindServicePoint(Uri address)
        {
            if (this.Proxy != null)
            {
                address = this.Proxy.GetProxy(address);
            }
            return ServicePointManager.FindServicePoint(address);
        }

        /// <summary>
        /// 表示Uri集合
        /// </summary>
        private class UriHashSet
        {
            /// <summary>
            /// 同步锁
            /// </summary>
            private readonly object syncRoot = new object();

            /// <summary>
            /// 站点地址
            /// </summary>
            private readonly HashSet<Uri> hashSet = new HashSet<Uri>(new UriComparer());

            /// <summary>
            /// 添加Uri
            /// </summary>
            /// <param name="uri"></param>
            /// <returns></returns>
            public bool Add(Uri uri)
            {
                lock (this.syncRoot)
                {
                    return this.hashSet.Add(uri);
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
}

#endif