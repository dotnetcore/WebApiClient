using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示http上下文
    /// </summary>
    public class HttpContext : HttpClientContext, IDisposable
    {
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
        public HttpResponseMessage? ResponseMessage { get; internal set; }

        /// <summary>
        /// http上下文
        /// </summary>
        /// <param name="context">服务上下文</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpContext(HttpClientContext context)
            : base(context.HttpClient, context.ServiceProvider, context.HttpApiOptions, context.OptionsName)
        {
            var requiredUri = context.HttpApiOptions.HttpHost ?? context.HttpClient.BaseAddress;
            this.RequestMessage = new HttpApiRequestMessage(requiredUri, context.HttpApiOptions.UseDefaultUserAgent);
            this.CancellationTokens = new List<CancellationToken>();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            this.RequestMessage?.Dispose();
        }
    }
}
