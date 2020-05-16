using System.Net.Http;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示参数值理解为HttpContent类型的特性
    /// 例如StringContent、ByteArrayContent、FormUrlEncodedContent等类型
    /// </summary>
    public class HttpContentAttribute : ApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param> 
        /// <exception cref="HttpApiInvalidOperationException"></exception>
        /// <returns></returns>
        public sealed override async Task OnRequestAsync(ApiParameterContext context)
        {
            var method = context.HttpContext.RequestMessage.Method;
            if (method == HttpMethod.Get || method == HttpMethod.Head)
            {
                var message = Resx.unsupported_SetContent.Format(method);
                throw new HttpApiInvalidOperationException(message);
            }
            await this.SetHttpContentAsync(context).ConfigureAwait(false);
        }

        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <exception cref="HttpApiInvalidOperationException"></exception>
        /// <returns></returns>
        protected virtual Task SetHttpContentAsync(ApiParameterContext context)
        {
            this.SetHttpContent(context);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param> 
        /// <exception cref="HttpApiInvalidOperationException"></exception>
        protected virtual void SetHttpContent(ApiParameterContext context)
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
