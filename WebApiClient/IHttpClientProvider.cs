using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 定义HttpClient提供者
    /// </summary>
    public interface IHttpClientProvider
    {
        /// <summary>
        /// 请求前获取HttpClient的实例
        /// </summary>
        /// <param name="context">请求上下文</param>
        /// <returns></returns>
        HttpClient GetHttpClient(ApiActionContext context);

        /// <summary>
        /// 在Http请求完成之后
        /// </summary>
        /// <param name="context">请求上下文</param>
        /// <param name="httpClient">httpClient实例</param>
        void OnRequestCompleted(ApiActionContext context, HttpClient httpClient);
    }
}
