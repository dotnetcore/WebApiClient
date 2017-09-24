using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示参数值作为x-www-form-urlencoded的字段
    /// </summary>
    public class FormFieldAttribute : HttpContentAttribute
    {
        /// <summary>
        /// 生成http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        protected override async Task<HttpContent> GenerateHttpContentAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var stringValue = parameter.Value == null ? null : parameter.Value.ToString();
            var keyValue = new KeyValuePair<string, string>(parameter.Name, stringValue);
            return await context.RequestMessage.Content.MergeKeyValuesAsync(new[] { keyValue });
        }
    }
}
