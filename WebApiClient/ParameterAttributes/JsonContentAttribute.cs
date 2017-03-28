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
    /// 表示application/json的http内容
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class JsonContentAttribute : HttpContentAttribute
    {
        /// <summary>
        /// 获取http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        protected override HttpContent GetHttpContent(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var json = parameter.Value == null ? null : context.HttpApiClient.JsonFormatter.Serialize(parameter.Value);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
