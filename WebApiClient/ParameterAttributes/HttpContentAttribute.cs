
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示参数为HttpContent或派生类型的特性
    /// 此特性不需要显示声明
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class HttpContentAttribute : ApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        public sealed override async Task BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var method = context.RequestMessage.Method;
            if (method == HttpMethod.Get || method == HttpMethod.Head)
            {
                return;
            }

            var httpContent = this.GetHttpContent(context, parameter);
            context.RequestMessage.Content = httpContent;
            await TaskExtend.CompletedTask;
        }

        /// <summary>
        /// 获取http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的属性</param>
        /// <returns></returns>
        protected virtual HttpContent GetHttpContent(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            return parameter.Value as HttpContent;
        }

        /// <summary>
        /// 格式化参数
        /// </summary>
        /// <param name="formater">格式化工具</param>
        /// <param name="encoding">编码</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        protected string FormatParameter(IStringFormatter formater, Encoding encoding, ApiParameterDescriptor parameter)
        {
            if (parameter.Value == null)
            {
                return null;
            }

            if (parameter.ParameterType == typeof(string))
            {
                return parameter.Value.ToString();
            }
            return formater.Serialize(parameter.Value, encoding);
        }
    }
}
