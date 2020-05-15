using System;
using System.Net.Http;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示http上下文
    /// </summary>
    public class HttpContext : Disposable
    {
        /// <summary>
        /// 获取关联的HttpClient实例
        /// </summary>
        public HttpClient HttpClient { get; }

        /// <summary>
        /// 获取服务提供者
        /// </summary>
        public IServiceProvider RequestServices { get; }

        /// <summary>
        /// 获取关联的HttpRequestMessage
        /// </summary>
        public HttpApiRequestMessage RequestMessage { get; }

        /// <summary>
        /// 获取关联的的HttpResponseMessage
        /// </summary>
        public HttpResponseMessage ResponseMessage { get; internal set; }

        /// <summary>
        /// http上下文
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="services"></param>
        /// <param name="httpHost"></param>
        public HttpContext(HttpClient httpClient, IServiceProvider services, Uri httpHost)
        {
            this.HttpClient = httpClient;
            this.RequestServices = services;
            this.RequestMessage = new HttpApiRequestMessage { RequestUri = httpHost };
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            this.RequestMessage?.Dispose();
        }
    }
}
