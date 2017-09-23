using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 表示将自身作为multipart/form-data的一个文本项
    /// </summary>
    public class MulitpartText : IApiParameterable
    {
        /// <summary>
        /// 文本内容
        /// </summary>
        private readonly string value;

        /// <summary>
        /// multipart/form-data的一个文本项
        /// </summary>     
        /// <param name="value">文本内容</param>
        public MulitpartText(string value)
        {
            this.value = value;
        }

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        async Task IApiParameterable.BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var method = context.RequestMessage.Method;
            if (method == HttpMethod.Get || method == HttpMethod.Head)
            {
                return;
            }

            var valueEncode = HttpUtility.UrlEncode(this.value, Encoding.UTF8);
            var httpContent = context.RequestMessage.Content.CastOrCreateMultipartContent();

            httpContent.AddText(parameter.Name, valueEncode);
            context.RequestMessage.Content = httpContent;

            await TaskExtend.CompletedTask;
        }
        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.value;
        }

        /// <summary>
        /// 从string类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator MulitpartText(string value)
        {
            return new MulitpartText(value);
        }
    }
}
