using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示将参数的值作为请求Url的特性
    /// 支持绝对或相对路径
    /// 一般放到第一个参数以防止将PathQuery的路径覆盖掉
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class UrlAttribute : ApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        public override Task BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            if (parameter.Value == null)
            {
                throw new ArgumentNullException(parameter.Name);
            }

            var url = new Uri(parameter.Value.ToString(), UriKind.RelativeOrAbsolute);
            if (url.IsAbsoluteUri == true)
            {
                context.RequestMessage.RequestUri = url;
            }
            else
            {
                var baseUrl = context.RequestMessage.RequestUri;
                context.RequestMessage.RequestUri = new Uri(baseUrl, url);
            }
            return TaskExtend.CompletedTask;
        }
    }
}
