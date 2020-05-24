using System.Net.Http;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示将参数值处理为请求Content的特性抽象
    /// </summary>
    public abstract class HttpContentAttribute : ApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param> 
        /// <exception cref="ApiInvalidConfigException"></exception>
        /// <returns></returns>
        public sealed override async Task OnRequestAsync(ApiParameterContext context)
        {
            var method = context.HttpContext.RequestMessage.Method;
            if (method == HttpMethod.Get || method == HttpMethod.Head)
            {
                var message = Resx.unsupported_SetContent.Format(method);
                throw new ApiInvalidConfigException(message);
            }
            await this.SetHttpContentAsync(context).ConfigureAwait(false);
        }

        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected abstract Task SetHttpContentAsync(ApiParameterContext context);
    }
}
