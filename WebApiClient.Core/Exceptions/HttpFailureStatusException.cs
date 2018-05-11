using System;
using System.Net;
using System.Net.Http;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 表示Http失败状态码异常
    /// </summary>
    public class HttpFailureStatusException : HttpRequestException
    {
        /// <summary>
        /// 获取状态码
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// 获取请求Api的上下文
        /// </summary>
        public ApiActionContext ApiActionContext { get; private set; }

        /// <summary>
        /// Http失败状态码异常
        /// </summary>
        /// <param name="statusCode">状态码</param>
        /// <param name="context">上下文</param>
        /// <param name="exception">异常</param>
        public HttpFailureStatusException(HttpStatusCode statusCode, ApiActionContext context, Exception exception)
            : base(exception?.Message, exception)
        {
            this.StatusCode = statusCode;
            this.ApiActionContext = context;
        }
    }
}
