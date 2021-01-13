using System;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示请求服务http绝对完整主机域名
    /// 例如http://www.abc.com/
    /// </summary> 
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public partial class HttpHostAttribute : IApiParameterAttribute
    {
        /// <summary>
        /// 请求服务http绝对完整主机域名      
        /// </summary> 
        [AttributeCtorUsage(AttributeTargets.Parameter)]
        public HttpHostAttribute()
        {
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public Task OnRequestAsync(ApiParameterContext context)
        {
            var hostValue = context.ParameterValue;
            if (hostValue == null)
            {
                return Task.CompletedTask;
            }

            var httpHost = ConvertToUri(hostValue);
            context.HttpContext.RequestMessage.ReplaceHttpHost(httpHost);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 转换为Uri
        /// </summary>
        /// <param name="hostValue">参数值</param>
        /// <exception cref="ApiInvalidConfigException"></exception>
        /// <returns></returns>
        private static Uri ConvertToUri(object hostValue)
        {
            if (hostValue is Uri httpHost)
            {
                return httpHost;
            }

            var uriString = hostValue.ToString();
            if (Uri.TryCreate(uriString, UriKind.Absolute, out httpHost))
            {
                return httpHost;
            }

            throw new ApiInvalidConfigException(Resx.parameter_CannotCvtUri.Format(uriString));
        }
    }
}
