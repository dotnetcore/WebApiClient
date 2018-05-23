using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WebApiClient
{
    /// <summary>
    /// 表示不支持处理的响应消息异常
    /// </summary>
    public class ApiReturnNotSupportedExteption : HttpRequestException
    {
        /// <summary>
        /// 获取响应消息
        /// </summary>
        public HttpResponseMessage ResponseMessage { get; private set; }

        /// <summary>
        /// 获取要转换的目标类型
        /// </summary>
        public Type ReturnDataType { get; private set; }

        /// <summary>
        /// 获取响应内容的Content-Type
        /// </summary>
        public MediaTypeHeaderValue ContentType
        {
            get => this.ResponseMessage.Content.Headers.ContentType;
        }

        /// <summary>
        /// 不支持处理的响应消息异常
        /// </summary>
        /// <param name="responseMessage">响应消息</param>
        /// <param name="returnDataType">反序列化的目标类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiReturnNotSupportedExteption(HttpResponseMessage responseMessage, Type returnDataType)
            : base(GetErrorMessage(responseMessage, returnDataType))
        {
            this.ResponseMessage = responseMessage;
            this.ReturnDataType = returnDataType;
        }

        /// <summary>
        /// 返回异常提示
        /// </summary>
        /// <param name="responseMessage">响应消息</param>
        /// <param name="returnDataType">反序列化的目标类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        private static string GetErrorMessage(HttpResponseMessage responseMessage, Type returnDataType)
        {
            if (responseMessage == null)
            {
                throw new ArgumentNullException(nameof(responseMessage));
            }

            return $"响应的内容不支持反序列化为{returnDataType}，请检查响应的ContentType和内容是否正确，或者强制为接口指定正确的ApiReturnAttribute";
        }
    }
}
