using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WebApiClientCore.Exceptions
{
    /// <summary>
    /// 表示不支持处理的响应消息异常
    /// </summary>
    public class ApiReturnNotSupportedExteption : HttpApiException
    {
        /// <summary>
        /// 获取响应消息
        /// </summary>
        public HttpResponseMessage ResponseMessage { get; }

        /// <summary>
        /// 获取要转换的目标类型
        /// </summary>
        public Type ReturnDataType { get; }


        /// <summary>
        /// 获取响应内容的Content-Type
        /// </summary>
        public MediaTypeHeaderValue ContentType
        {
            get => this.ResponseMessage.Content.Headers.ContentType;
        }

        /// <summary>
        /// 获取异常提示信息
        /// </summary>
        public override string Message => $"不支持将ContentType为{this.ContentType}的内容反序列化为{this.ReturnDataType}";

        /// <summary>
        /// 不支持处理的响应消息异常
        /// </summary>
        /// <param name="responseMessage">响应消息</param>
        /// <param name="returnDataType">反序列化的目标类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiReturnNotSupportedExteption(HttpResponseMessage responseMessage, Type returnDataType)
        {
            this.ResponseMessage = responseMessage ?? throw new ArgumentNullException(nameof(responseMessage));
            this.ReturnDataType = returnDataType ?? throw new ArgumentNullException(nameof(returnDataType));
        }
    }
}
