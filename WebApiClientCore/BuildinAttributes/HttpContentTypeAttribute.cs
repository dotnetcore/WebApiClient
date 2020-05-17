using System.Net.Http;
using WebApiClientCore.Attributes;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示参数内容为HttpContent类型的处理特性
    /// </summary>
    class HttpContentTypeAttribute : HttpContentAttribute
    {
        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param> 
        /// <exception cref="HttpApiInvalidOperationException"></exception>
        protected override void SetHttpContent(ApiParameterContext context)
        {
            if (context.HttpContext.RequestMessage.Content != null)
            {
                var message = Resx.parameter_MustPutForward.Format(context.Parameter.Member);
                throw new HttpApiInvalidOperationException(message);
            }

            if (context.ParameterValue != null)
            {
                if (context.ParameterValue is HttpContent httpContent)
                {
                    context.HttpContext.RequestMessage.Content = httpContent;
                }
                else
                {
                    var message = Resx.parameter_MustbeHttpContenType.Format(context.Parameter.Member);
                    throw new HttpApiInvalidOperationException(message);
                }
            }
        }
    }
}
