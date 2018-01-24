#if NET45

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
        /// 发送次数
        /// </summary>
        private int sendTimes = 0;

        /// <summary>
        /// 最大连接数设置器
        /// </summary>
        private static readonly PropertySetter MaxConnectionsPerServer;

        /// <summary>
        /// 静态构造器
        /// </summary>
        static DefaultHttpClientHandler()
        {
            // 支持st20的framework具有MaxConnectionsPerServer属性
            var property = typeof(HttpClientHandler).GetProperty("MaxConnectionsPerServer", typeof(int));
            if (property != null)
            {
                MaxConnectionsPerServer = new PropertySetter(property);
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

            if (MaxConnectionsPerServer != null)
            {
                MaxConnectionsPerServer.Invoke(this, HttpApiClient.ConnectionLimit);
            }
            else
            {
                ServicePointManager.DefaultConnectionLimit = HttpApiClient.ConnectionLimit;
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
    }
}

#endif