using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 表示将自身请求Url
    /// 支持绝对或相对路径
    /// 一般放到第一个参数以防止将PathQuery的路径覆盖掉
    /// 不可继承
    /// </summary>
    public sealed class Url : IApiParameterable
    {
        /// <summary>
        /// 请求的url
        /// </summary>
        private readonly Uri url;

        /// <summary>
        /// 将自身请求Url
        /// </summary>
        /// <param name="url">请求url</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UriFormatException"></exception>
        public Url(string url)
        {
            this.url = new Uri(url, UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// 将自身请求Url
        /// </summary>
        /// <param name="url">请求url</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Url(Uri url)
        {
            if (url == null)
            {
                throw new ArgumentNullException();
            }
            this.url = url;
        }

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        Task IApiParameterable.BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            if (this.url.IsAbsoluteUri == true)
            {
                context.RequestMessage.RequestUri = this.url;
            }
            else
            {
                var baseUrl = context.RequestMessage.RequestUri;
                context.RequestMessage.RequestUri = new Uri(baseUrl, this.url);
            }
            return TaskExtend.CompletedTask;
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.url.ToString();
        }
    }
}
