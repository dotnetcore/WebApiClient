using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示请求Api的上下文
    /// </summary>
    public class ApiActionContext : Disposable
    {
        /// <summary>
        /// 获取httpApi代理类实例
        /// </summary>
        public IHttpApi HttpApi { get; }

        /// <summary>
        /// 获取关联的HttpClient实例
        /// </summary>
        public HttpClient HttpClient { get; }

        /// <summary>
        /// 获取配置选项
        /// </summary>
        public HttpApiOptions Options { get; }

        /// <summary>
        /// 获取服务提供者
        /// </summary>
        public IServiceProvider RequestServices { get; }

        /// <summary>
        /// 获取关联的ApiAction描述
        /// </summary>
        public ApiActionDescriptor ApiAction { get; }

        /// <summary>
        /// 获取请求参数值
        /// </summary>
        public IReadOnlyList<object> Arguments { get; }

        /// <summary>
        /// 获取关联的HttpRequestMessage
        /// </summary>
        public HttpApiRequestMessage RequestMessage { get; }


        /// <summary>
        /// 获取本次请求相关的自定义数据的存储和访问容器
        /// </summary>
        public Tags Tags { get; } = new Tags();

        /// <summary>
        /// 获取请求取消令牌集合
        /// 这些令牌将被连接起来
        /// </summary>
        public IList<CancellationToken> CancellationTokens { get; } = new List<CancellationToken>();



        /// <summary>
        /// 获取关联的的HttpResponseMessage
        /// </summary>
        public HttpResponseMessage ResponseMessage { get; internal set; }

        /// <summary>
        /// 获取调用Api得到的结果
        /// </summary>
        public object Result { get; internal set; }

        /// <summary>
        /// 获取调用Api产生的异常
        /// </summary>
        public Exception Exception { get; internal set; }


        /// <summary>
        /// 请求Api的上下文
        /// </summary>
        /// <param name="httpApi"></param>
        /// <param name="httpClient"></param>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <param name="apiAction"></param>
        /// <param name="arguments"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiActionContext(IHttpApi httpApi, HttpClient httpClient, IServiceProvider services, HttpApiOptions options, ApiActionDescriptor apiAction, IEnumerable<object> arguments)
        {
            this.HttpApi = httpApi ?? throw new ArgumentNullException(nameof(httpApi));
            this.HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.RequestServices = services ?? throw new ArgumentNullException(nameof(services));
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
            this.ApiAction = apiAction ?? throw new ArgumentNullException(nameof(apiAction));
            this.Arguments = arguments.ToReadOnlyList();
            this.RequestMessage = new HttpApiRequestMessage { RequestUri = options.HttpHost };
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否也释放托管资源</param>
        protected override void Dispose(bool disposing)
        {
            this.RequestMessage?.Dispose();
        }
    }
}
