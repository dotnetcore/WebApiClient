
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示请求的http内容体抽象类
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public abstract class HttpContentAttribute : ApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        public sealed override void BeforeRequest(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            if (context.RequestMessage.Method == HttpMethod.Get)
            {
                return;
            }

            var httpContent = this.GetHttpContent(context, parameter);
            context.RequestMessage.Content = httpContent;
        }

        /// <summary>
        /// 获取http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的属性</param>
        /// <returns></returns>
        protected abstract HttpContent GetHttpContent(ApiActionContext context, ApiParameterDescriptor parameter);
    }
}
