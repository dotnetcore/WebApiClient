using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示将参数值作为请求url的特性
    /// 支持绝对或相对路径
    /// 一般放到第一个参数以防止将PathQuery的路径覆盖掉
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class UrlAttribute : Attribute, IApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <exception cref="ApiConfigException"></exception>
        /// <returns></returns>
        public virtual Task BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            if (parameter.Value == null)
            {
                throw new ArgumentNullException(parameter.Name);
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
                    throw new ApiConfigException("请配置HttpConfig.HttpHost或使用HttpHostAttribute特性，否则必须使用绝对路径");
                }
                context.RequestMessage.RequestUri = new Uri(baseUri, relative);
            }
            return ApiTask.CompletedTask;
        }
    }
}
