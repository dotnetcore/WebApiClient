using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 定义HttpClient的接口
    /// </summary>
    public interface IHttpClient : IDisposable
    {
        /// <summary>
        /// 获取关联的Http处理对象
        /// </summary>
        HttpClientHandler Handler { get; }

        /// <summary>
        /// 获取默认的请求头管理对象
        /// </summary>
        HttpRequestHeaders DefaultRequestHeaders { get; }

        /// <summary>
        /// 获取或设置请求超时时间
        /// </summary>
        TimeSpan Timeout { get; set; }

        /// <summary>
        /// 获取或设置最大回复内容长度
        /// </summary>
        long MaxResponseContentBufferSize { get; set; }

        /// <summary>
        /// 设置代理
        /// </summary>
        /// <param name="proxy">代理，为null则清除代理</param>
        /// <returns></returns>
        bool SetProxy(IWebProxy proxy);

        /// <summary>
        /// 取消正在挂起的请求
        /// </summary>
        void CancelPendingRequests();

        /// <summary>
        /// 异步发送请求
        /// </summary>
        /// <param name="request">请求消息</param>
        /// <returns></returns>
        Task<HttpResponseMessage> SendAsync(HttpApiRequestMessage request);
    }
}
