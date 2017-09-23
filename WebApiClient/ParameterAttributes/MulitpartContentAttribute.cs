using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示参数值为multipart/form-data表单或表单的一个项
    /// Api的所有参数都可以注明一次
    /// 支持单一值类型如string、int、guid、枚举等，以及他们的可空类型或集合
    /// 支持POCO类型、IDictionaryOf(string,string)类型、IDictionaryOf(string,object)类型
    /// </summary>
    public class MulitpartContentAttribute : HttpContentAttribute
    {
        /// <summary>
        /// 生成http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的属性</param>
        /// <returns></returns>
        protected override HttpContent GenerateHttpContent(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var httpContent = context.RequestMessage.Content.CastOrCreateMultipartContent();
            var q = from kv in base.FormatParameter(parameter)
                    select new MulitpartTextContent(kv.Key, kv.Value);

            foreach (var item in q)
            {
                httpContent.Add(item);
            }
            return httpContent;
        }
    }
}
