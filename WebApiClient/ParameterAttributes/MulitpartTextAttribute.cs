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
    /// 表示参数值作为multipart/form-data表单的一个文本项
    /// </summary>
    public class MulitpartTextAttribute : HttpContentAttribute
    {
        /// <summary>
        /// 生成http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        protected override HttpContent GenerateHttpContent(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var stringValue = parameter.Value == null ? null : parameter.Value.ToString();
            var httpContent = context.RequestMessage.Content.CastOrCreateMultipartContent();
            httpContent.AddText(parameter.Name, stringValue);
            return httpContent;
        }
    }
}
