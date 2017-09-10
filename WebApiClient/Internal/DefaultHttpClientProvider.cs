using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 默认HttpClient提供者
    /// httpClient单例
    /// </summary>
    class DefaultHttpClientProvider : IHttpClientProvider, IDisposable
    {
        /// <summary>
        /// http客户端
        /// </summary>
        private readonly HttpClient client;

        /// <summary>
        /// HttpClient提供者
        /// </summary>
        public DefaultHttpClientProvider()
        {
            this.client = new HttpClient(new HttpClientHandler(), true);
        }

        /// <summary>
        /// 请求前获取HttpClient的实例
        /// </summary>
        /// <returns></returns>
        public HttpClient CreateHttpClient()
        {
            return this.client;
        }

        /// <summary>
        /// 请求完成释放HttpClient实例
        /// </summary>
        /// <param name="httpClient">httpClient实例</param>
        public void DisposeHttpClient(HttpClient httpClient)
        {
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.client.Dispose();
        }
    }
}
