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
                    let value = kv.Value == null ? string.Empty : HttpUtility.UrlEncode(kv.Value, encoding)
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
