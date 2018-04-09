using System;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示将参数值作为请求url的特性  
    /// 要求必须修饰于第一个参数
    /// 支持绝对或相对路径
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class UrlAttribute : Attribute, IApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <exception cref="HttpApiConfigException"></exception>
        /// <returns></returns>
        public Task BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            if (parameter.Value == null)
            {
                throw new ArgumentNullException(parameter.Name);
            }

            if (parameter.Index > 0)
            {
                throw new HttpApiConfigException(this.GetType().Name + "必须修饰于第一个参数");
            }

            var relative = new Uri(parameter.ToString(), UriKind.RelativeOrAbsolute);
            if (relative.IsAbsoluteUri == true)
            {
                context.RequestMessage.RequestUri = relative;
            }
            else
            {
                var baseUri = context.RequestMessage.RequestUri;
                if (baseUri == null)
                {
                    throw new HttpApiConfigException("请配置HttpHost或者Url使用绝对路径");
                }
                context.RequestMessage.RequestUri = new Uri(baseUri, relative);
            }
            return ApiTask.CompletedTask;
        }
    }
}
