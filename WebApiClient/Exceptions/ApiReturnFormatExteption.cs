using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WebApiClient
{
    /// <summary>
    /// 表示接口的返回类型序列化异常
    /// </summary>
    public class ApiReturnFormatExteption : HttpRequestException
    {
        /// <summary>
        /// 获取响应消息
        /// </summary>
        public HttpResponseMessage ResponseMessage { get; private set; }

        /// <summary>
        /// 获取响应内容的Content-Type
        /// </summary>
        public MediaTypeHeaderValue ContentType
        {
            get => this.ResponseMessage.Content.Headers.ContentType;
        }

        /// <summary>
        /// Http失败状态码异常
        /// </summary>
        /// <param name="responseMessage">响应消息</param>
        /// <param name="targetType">要反序列化的目标类型</param>  
        /// <exception cref="ArgumentNullException"></exception>
        public ApiReturnFormatExteption(HttpResponseMessage responseMessage, Type targetType)
            : base(GetErrorMessage(responseMessage, targetType))
        {
            this.ResponseMessage = responseMessage;
        }

        /// <summary>
        /// 返回异常提示
        /// </summary>
        /// <param name="responseMessage">响应消息</param>
        /// <param name="targetType">要反序列化的目标类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        private static string GetErrorMessage(HttpResponseMessage responseMessage, Type targetType)
        {
            if (responseMessage == null)
            {
                throw new ArgumentNullException(nameof(responseMessage));
            }
            var contentType = responseMessage.Content.Headers.ContentType?.MediaType ?? "<NULL>";
            return $"响应的内容不支持反序列化为{targetType}，请检查响应的ContentType和内容是否正确，或者强制为接口指定正确的ApiReturnAttribute";
        }
    }
}
