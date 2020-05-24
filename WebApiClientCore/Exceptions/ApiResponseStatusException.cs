using System;
using System.Net;
using System.Net.Http;

namespace WebApiClientCore.Exceptions
{
    /// <summary>
    /// 表示接口响应状态码异常
    /// </summary>
    public class ApiResponseStatusException : ApiException
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
        /// 获取异常提示消息
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
        /// 接口响应状态码异常
        /// </summary> 
        /// <param name="responseMessage">响应消息</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiResponseStatusException(HttpResponseMessage responseMessage)
        {
            this.ResponseMessage = responseMessage;
        }
    }
}
