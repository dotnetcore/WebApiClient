using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 定义HttpClient的上下文
    /// </summary>
    public interface IHttpClientContext
    {
        /// <summary>
        /// 获取HttpClient实例
        /// </summary>
        HttpClient HttpClient { get; }

        /// <summary>
        /// 获取HttpClient处理者
        /// </summary>
        HttpClientHandler HttpClientHandler { get; }
    }
}
