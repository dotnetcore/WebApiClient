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
    }
}
