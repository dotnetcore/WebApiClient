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
        /// 获取http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的属性</param>
        /// <returns></returns>
        protected override HttpContent GetHttpContent(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var encoding = Encoding.UTF8;
            var httpContent = this.GetHttpContentFromContext(context);

            var q = from kv in base.FormatParameter(parameter)
                    let value = HttpUtility.UrlEncode(kv.Value, encoding)
                    select new MulitpartTextContent(value, kv.Key);

            foreach (var item in q)
            {
                httpContent.Add(item);
            }
            return httpContent;
        }

        /// <summary>
        /// 从上下文获取已有MultipartFormDataContent
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        private MultipartContent GetHttpContentFromContext(ApiActionContext context)
        {
            var httpContent = context.RequestMessage.Content as MultipartContent;
            if (httpContent == null)
            {
                httpContent = new MultipartFormDataContent();
            }
            return httpContent;
        }
    }
}
