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
        /// <param name="uriValue"></param>
        /// <exception cref="ApiInvalidConfigException"></exception>
        /// <returns></returns>
        public static Uri? CreateRequestUri(Uri? baseUri, Uri uriValue)
        {
            if (uriValue.IsAbsoluteUri == false)
            {
                return CreateUriByRelative(baseUri, uriValue);
            }

            if (uriValue.AbsolutePath == "/")
            {
                return CreateUriByAbsolute(uriValue, baseUri);
            }

            return uriValue;
        }

        /// <summary>
        /// 创建uri
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="relative"></param>
        /// <returns></returns>
        private static Uri CreateUriByRelative(Uri? baseUri, Uri relative)
        {
            if (baseUri == null)
            {
                return relative;
            }

            if (baseUri.IsAbsoluteUri == true)
            {
                return new Uri(baseUri, relative);
            }

            return relative;
        }

        /// <summary>
        /// 创建uri
        /// 参数值的uri是绝对uir，且只有根路径
        /// </summary>
        /// <param name="absolute"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        private static Uri CreateUriByAbsolute(Uri absolute, Uri? uri)
        {
            if (uri == null)
            {
                return absolute;
            }

            var relative = uri.ToRelativeUri();
            return relative == "/" ? absolute : new Uri(absolute, relative);
        }
    }
}
