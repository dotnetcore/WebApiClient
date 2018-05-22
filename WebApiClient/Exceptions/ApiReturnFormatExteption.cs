using System;
using System.Net.Http;
using System.Net.Http.Headers;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 表示接口的返回类型序列化异常
    /// </summary>
    public class ApiReturnFormatExteption : HttpRequestException
    {
        /// <summary>
        /// 获取请求Api的上下文
        /// </summary>
        public ApiActionContext ApiActionContext { get; private set; }

        /// <summary>
        /// 获取响应内容的Content-Type
        /// </summary>
        public MediaTypeHeaderValue ContentType
        {
            get => this.ApiActionContext.RequestMessage.Content.Headers.ContentType;
        }

        /// <summary>
        /// 接口的返回类型序列化异常
        /// </summary>
        /// <param name="context">请求Api的上下文</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiReturnFormatExteption(ApiActionContext context)
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

            var dataType = context.ApiActionDescriptor.Return.DataType;
            return $"响应的内容不支持反序列化为{dataType}，请检查响应的ContentType和内容是否正确，或者强制为接口指定正确的ApiReturnAttribute";
        }
    }
}
