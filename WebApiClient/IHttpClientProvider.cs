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
        /// <returns></returns>
        HttpClient CreateHttpClient();

        /// <summary>
        /// 请求完成释放HttpClient实例
        /// </summary>
        /// <param name="httpClient">httpClient实例</param>
        void DisposeHttpClient(HttpClient httpClient);
    }
}
