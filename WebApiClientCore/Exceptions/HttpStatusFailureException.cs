using System;
using System.Net;
using System.Net.Http;

namespace WebApiClientCore.Exceptions
{
    /// <summary>
    /// 表示Http失败状态码异常
    /// </summary>
    public class HttpStatusFailureException : HttpApiException
    {
        /// <summary>
        /// 获取响应消息
        /// </summary>
        public HttpResponseMessage ResponseMessage { get; }

        /// <summary>
        /// 获取响应状态码
        /// </summary>
        public HttpStatusCode StatusCode => this.ResponseMessage.StatusCode;

        /// <summary>
        /// 返回异常提示
        /// </summary>
        public override string Message
        {
            get
            {
                var code = (int)this.ResponseMessage.StatusCode;
                var reason = this.ResponseMessage.ReasonPhrase;
                return Resx.failure_StatusCode.Format(code, reason);
            }
        }

        /// <summary>
        /// Http失败状态码异常
        /// </summary> 
        /// <param name="responseMessage"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpStatusFailureException(HttpResponseMessage responseMessage)
        {
            this.ResponseMessage = responseMessage;
        }
    }
}
