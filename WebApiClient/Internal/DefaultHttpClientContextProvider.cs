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
    /// HttpClientContext提供者
    /// </summary>
    class DefaultHttpClientContextProvider : IHttpClientContextProvider
    {
        /// <summary>
        /// 在请求前将创建IHttpClientContext
        /// </summary>
        /// <param name="context">请求上下文</param>
        /// <returns></returns>
        public IHttpClientContext CreateHttpClientContext(ApiActionContext context)
        {
            return new HttpClientContext();
        }

        /// <summary>
        /// <summary>
        /// 在Http请求完成之后释放HttpClientContext 
        /// </summary>
        /// <param name="context">HttpClient上下文</param>
        public void DisponseHttpClientContext(IHttpClientContext context)
        {
            context.HttpClient.Dispose();
        }

        /// <summary>
        /// 表示HttpClient上下文
        /// </summary>
        class HttpClientContext : IHttpClientContext
        {
            /// <summary>
            /// 获取HttpClient实例
            /// </summary>
            public HttpClient HttpClient { get; private set; }

            /// <summary>
            /// 获取HttpClient处理者
            /// </summary>
            public HttpClientHandler HttpClientHandler { get; private set; }

            /// <summary>
            /// HttpClient上下文
            /// </summary>
            public HttpClientContext()
            {
                this.HttpClientHandler = new HttpClientHandler();
                this.HttpClient = new HttpClient(this.HttpClientHandler, true);
            }
        }
    }
}
