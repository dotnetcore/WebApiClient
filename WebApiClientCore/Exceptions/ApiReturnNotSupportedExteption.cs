using System;

namespace WebApiClientCore.Exceptions
{
    /// <summary>
    /// 表示不支持处理的响应消息异常
    /// </summary>
    public class ApiReturnNotSupportedExteption : HttpApiException
    {
        /// <summary>
        /// 获取请求上下文
        /// </summary>
        public ApiRequestContext Context { get; }

        /// <summary>
        /// 获取异常提示信息
        /// </summary>
        public override string Message
        {
            get
            {
                var contentType = this.Context.HttpContext.ResponseMessage?.Content.Headers.ContentType.ToString();
                return Resx.unspported_ContentType.Format(contentType, this.Context.ApiAction.Return.DataType.Type);
            }
        }

        /// <summary>
        /// 不支持处理的响应消息异常
        /// </summary>
        /// <param name="context"></param> 
        /// <exception cref="ArgumentNullException"></exception>
        public ApiReturnNotSupportedExteption(ApiRequestContext context)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }
}
