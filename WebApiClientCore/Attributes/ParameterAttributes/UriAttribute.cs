using System;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示将参数值作为请求uri的特性  
    /// 要求必须修饰于第一个参数
    /// 支持绝对或相对路径
    /// </summary>
    public class UriAttribute : ApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <exception cref="ApiInvalidConfigException"></exception>
        /// <returns></returns>
        public override Task OnRequestAsync(ApiParameterContext context)
        {
            if (context.ParameterValue == null)
            {
                throw new ArgumentNullException(context.Parameter.Name);
            }

            if (context.Parameter.Index > 0)
            {
                throw new ApiInvalidConfigException(Resx.invalid_UriAttribute);
            }

            if (!(context.ParameterValue is Uri uri))
            {
                uri = new Uri(context.ParameterValue.ToString(), UriKind.RelativeOrAbsolute);
            }

            if (uri.IsAbsoluteUri == true)
            {
                context.HttpContext.RequestMessage.RequestUri = uri;
            }
            else
            {
                var baseUri = context.HttpContext.RequestMessage.RequestUri;
                if (baseUri == null)
                {
                    throw new ApiInvalidConfigException(Resx.required_HttpHost);
                }
                context.HttpContext.RequestMessage.RequestUri = new Uri(baseUri, uri);
            }
            return Task.CompletedTask;
        }
    }
}
