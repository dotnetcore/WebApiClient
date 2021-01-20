using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示http上下文
    /// </summary>
    public class HttpContext : HttpClientContext
    {
        /// <summary>
        /// 获取或设置指示请求完成选项
        /// </summary>
        public HttpCompletionOption? CompletionOption { get; set; }

        /// <summary>
        /// 获取请求取消令牌集合
        /// </summary>
        public IList<CancellationToken> CancellationTokens { get; }

        /// <summary>
        /// 获取请求消息
        /// </summary>
        public HttpApiRequestMessage RequestMessage { get; }

        /// <summary>
        /// 获取响应消息
        /// </summary>
        public HttpResponseMessage? ResponseMessage { get; set; }


        /// <summary>
        /// http上下文
        /// </summary>
        /// <param name="context">服务上下文</param>
        /// <param name="requestMessage"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpContext(HttpClientContext context, HttpApiRequestMessage requestMessage)
            : base(context.HttpClient, context.ServiceProvider, context.HttpApiOptions, context.OptionsName)
        {
            this.RequestMessage = requestMessage;
            this.CancellationTokens = new List<CancellationToken>();
        }
    }
}
