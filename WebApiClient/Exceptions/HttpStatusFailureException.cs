using System;
using System.Net;
using System.Net.Http;

namespace WebApiClient
{
    /// <summary>
    /// 表示Http失败状态码异常
    /// </summary>
    public class HttpStatusFailureException : HttpRequestException
    {
        /// <summary>
        /// 获取响应消息
        /// </summary>
        public HttpResponseMessage ResponseMessage { get; private set; }

        /// <summary>
        /// 获取响应状态码
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get => this.ResponseMessage.StatusCode;
        }

        /// <summary>
        /// Http失败状态码异常
        /// </summary>
        /// <param name="responseMessage">响应消息</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpStatusFailureException(HttpResponseMessage responseMessage)
           : base(GetErrorMessage(responseMessage))
        {
            this.ResponseMessage = responseMessage;
        }

        /// <summary>
        /// 返回异常提示
        /// </summary>
        /// <param name="responseMessage">响应消息</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        private static string GetErrorMessage(HttpResponseMessage responseMessage)
        {
            if (responseMessage == null)
            {
                throw new ArgumentNullException(nameof(responseMessage));
            }

            var code = (int)responseMessage.StatusCode;
            var reason = responseMessage.ReasonPhrase;
            return $"服务器响应了错误的的http状态码：{code} {reason}";
        }
    }
}
