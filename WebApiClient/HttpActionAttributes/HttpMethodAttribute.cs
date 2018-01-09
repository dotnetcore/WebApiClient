using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示http请求方法描述特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    [DebuggerDisplay("Method = {Method}")]
    public abstract class HttpMethodAttribute : ApiActionAttribute
    {
        /// <summary>
        /// 获取请求方法
        /// </summary>
        public HttpMethod Method { get; private set; }

        /// <summary>
        /// 获取请求相对路径
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// 获取顺序排序索引
        /// 优先级最高
        /// </summary>
        public override int OrderIndex
        {
            get
            {
                return int.MinValue + 1;
            }
        }


        /// <summary>
        /// http请求方法描述特性
        /// </summary>
        /// <param name="method">请求方法</param>
        public HttpMethodAttribute(HttpMethod method)
            : this(method, null)
        {
        }

        /// <summary>
        /// http请求方法描述特性
        /// </summary>
        /// <param name="method">请求方法</param>
        /// <param name="path">请求相对路径</param>
        public HttpMethodAttribute(HttpMethod method, string path)
        {
            this.Method = method;
            this.Path = path;
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override Task BeforeRequestAsync(ApiActionContext context)
        {
            var baseUrl = context.RequestMessage.RequestUri;
            if (baseUrl == null)
            {
                throw new UriFormatException("请设置配置项的HttpHost或添加HttpHostAttribute");
            }

            context.RequestMessage.Method = this.Method;
            if (string.IsNullOrEmpty(this.Path) == false)
            {
                context.RequestMessage.RequestUri = new Uri(baseUrl, this.Path);
            }
            return ApiTask.CompletedTask;
        }
    }
}
