using System.Net.Http;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore.Implementations.TypeAttributes
{
    /// <summary>
    /// 表示参数内容为HttpContent类型的处理特性
    /// </summary>
    sealed class HttpContentTypeAttribute : IApiParameterAttribute
    {
        /// <summary>
        /// 请求前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public Task OnRequestAsync(ApiParameterContext context)
        {
            if (context.HttpContext.RequestMessage.Content != null)
            {
                var message = Resx.parameter_MustPutForward.Format(context.Parameter.Member);
                throw new ApiInvalidConfigException(message);
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
                    throw new ApiInvalidConfigException(message);
                }
            }

            return Task.CompletedTask;
        }
    }
}
