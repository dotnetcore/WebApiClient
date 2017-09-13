using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// HttpClient提供者
    /// </summary>
    public class HttpClientProvider : IHttpClientProvider, IDisposable
    {
        /// <summary>
        /// http客户端
        /// </summary>
        private HttpClient client;

        /// <summary>
        /// 锁
        /// </summary>
        private readonly object syncRoot = new object();

        /// <summary>
        /// 获取HttpClient实例获取方式
        /// </summary>
        public InstanceType ClientInstanceType { get; private set; }

        /// <summary>
        /// HttpClient提供者
        /// </summary>
        /// <param name="clientInstanceType">HttpClient实例获取方式</param>
        public HttpClientProvider(InstanceType clientInstanceType)
        {
            this.ClientInstanceType = clientInstanceType;
        }

        /// <summary>
        /// 请求前获取HttpClient的实例
        /// </summary>
        /// <param name="context">请求上下文</param>
        /// <returns></returns>
        HttpClient IHttpClientProvider.GetHttpClient(ApiActionContext context)
        {
            var httpClient = this.GetHttpClient(context);
            this.OnHttpRequest(context, httpClient);
            return httpClient;
        }

        /// <summary>
        /// 请求前获取HttpClient的实例
        /// </summary>
        /// <param name="context">请求上下文</param>
        /// <returns></returns>
        private HttpClient GetHttpClient(ApiActionContext context)
        {
            if (this.ClientInstanceType == InstanceType.InstancePerRequest)
            {
                return this.CreateHttpClient(context);
            }

            lock (this.syncRoot)
            {
                if (this.client == null)
                {
                    this.client = this.CreateHttpClient(context);
                }
                return this.client;
            }
        }

        /// <summary>
        /// 请求完成释放HttpClient实例
        /// </summary>
        /// <param name="context">请求上下文</param>
        /// <param name="httpClient">httpClient实例</param>
        void IHttpClientProvider.OnRequestCompleted(ApiActionContext context, HttpClient httpClient)
        {
            if (this.ClientInstanceType == InstanceType.InstancePerRequest)
            {
                httpClient.Dispose();
            }
        }

        /// <summary>
        /// 创建Http客户端的实例
        /// </summary>
        /// <param name="context">请求上下文</param>
        /// <returns></returns>
        protected virtual HttpClient CreateHttpClient(ApiActionContext context)
        {
            return new HttpClient(new HttpClientHandler(), true);
        }

        /// <summary>
        /// 在http请求之前
        /// </summary>
        /// <param name="context">请求上下文</param>
        /// <param name="httpClient">httpClient实例</param>
        protected virtual void OnHttpRequest(ApiActionContext context, HttpClient httpClient)
        {
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (this.client != null)
            {
                this.client.Dispose();
            }
        }
    }
}
