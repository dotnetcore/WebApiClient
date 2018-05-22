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
        /// 获取请求Api的上下文
        /// </summary>
        public ApiActionContext ApiActionContext { get; private set; }

        /// <summary>
        /// 获取响应状态码
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get => this.ApiActionContext.ResponseMessage.StatusCode;
        }

        /// <summary>
        /// Http失败状态码异常
        /// </summary>
        /// <param name="context">上下文</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpFailureStatusException(ApiActionContext context)
           : base(GetErrorMessage(context))
        {
            this.ApiActionContext = context;
        }


        /// <summary>
        /// 返回异常提示
        /// </summary>
        /// <param name="context">请求Api的上下文</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        private static string GetErrorMessage(ApiActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var statusCode = context.ResponseMessage.StatusCode;
            return $"响应的http状态码不成功：{(int)statusCode} {statusCode}";
        }
    }
}
