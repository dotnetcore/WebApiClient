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
            var uri = context.HttpContext.RequestMessage.RequestUri;
            if (uri == null)
            {
                throw new ApiInvalidConfigException(Resx.required_HttpHost);
            }

            var host = GetHttpHost(context.ParameterValue, uri.Scheme); 
            context.HttpContext.RequestMessage.ReplaceHttpHost(host);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 获取httphost
        /// </summary>
        /// <param name="value">参数值</param>
        /// <param name="defaultScheme"></param>
        /// <returns></returns>
        private static Uri GetHttpHost(object? value, string defaultScheme)
        {
            if (value is Uri host)
            {
                if (host.IsAbsoluteUri == true)
                {
                    return host;
                }
                throw new UriFormatException(Resx.required_AbsoluteUri.Format(nameof(HttpHostAttribute)));
            }

            var uri = value?.ToString();
            if (Uri.TryCreate(uri, UriKind.Absolute, out host))
            {
                return host;
            }

            return new Uri($"{defaultScheme}://{uri}");
        }
    }
}
