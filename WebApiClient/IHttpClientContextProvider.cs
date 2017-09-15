using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 定义HttpClientContext提供者
    /// </summary>
    public interface IHttpClientContextProvider
    {
        /// <summary>
        /// 在请求前将创建IHttpClientContext
        /// </summary>
        /// <param name="context">请求上下文</param>
        /// <returns></returns>
        IHttpClientContext CreateHttpClientContext(ApiActionContext context);

        /// <summary>
        /// 在Http请求完成之后释放HttpClientContext 
        /// </summary>
        /// <param name="context">HttpClient上下文</param>
        void DisponseHttpClientContext(IHttpClientContext context);
    }
}
