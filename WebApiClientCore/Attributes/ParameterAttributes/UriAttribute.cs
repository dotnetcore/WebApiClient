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
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ApiInvalidConfigException"></exception>
        /// <returns></returns>
        public override Task OnRequestAsync(ApiParameterContext context)
        {
            if (context.Parameter.Index > 0)
            {
                throw new ApiInvalidConfigException(Resx.invalid_UriAttribute);
            }

            var uriValue = context.ParameterValue;
            if (uriValue == null)
            {
                throw new ArgumentNullException(context.ParameterName);
            }

            var uri = ConvertToUri(uriValue);
            var baseUri = context.HttpContext.RequestMessage.RequestUri;
            var requestUri = CreateRequestUri(baseUri, uri);
            context.HttpContext.RequestMessage.RequestUri = requestUri;

            return Task.CompletedTask;
        }


        /// <summary>
        /// 将参数值转换为Uri
        /// </summary>
        /// <param name="uriValue"></param>
        /// <exception cref="ApiInvalidConfigException"></exception>
        /// <returns></returns>
        private static Uri ConvertToUri(object uriValue)
        {
            if (uriValue is Uri uri)
            {
                return uri;
            }

            var uriString = uriValue.ToString();
            if (Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out uri))
            {
                return uri;
            }

            throw new ApiInvalidConfigException(Resx.parameter_CannotCvtUri.Format(uriString));
        }

        /// <summary>
        /// 创建请求URL
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="uri"></param>
        /// <exception cref="ApiInvalidConfigException"></exception>
        /// <returns></returns>
        private static Uri? CreateRequestUri(Uri? baseUri, Uri uri)
        {
            if (uri.IsAbsoluteUri == true)
            {
                return uri;
            }

            if (baseUri == null)
            {
                throw new ApiInvalidConfigException(Resx.required_HttpHost);
            }

            return new Uri(baseUri, uri);
        }
    }
}
