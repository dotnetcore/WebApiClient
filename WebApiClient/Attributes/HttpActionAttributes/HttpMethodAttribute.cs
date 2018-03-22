using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示http请求方法描述特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    [DebuggerDisplay("Method = {Method}")]
    public class HttpMethodAttribute : ApiActionAttribute
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
        /// <param name="path">请求绝对或相对路径</param>
        public HttpMethodAttribute(HttpMethod method, string path)
        {
            this.Method = method;
            this.Path = path;
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <exception cref="HttpApiConfigException"></exception>
        /// <returns></returns>
        public override Task BeforeRequestAsync(ApiActionContext context)
        {
            var baseUri = context.RequestMessage.RequestUri;
            var relative = string.IsNullOrEmpty(this.Path) ? null : new Uri(this.Path, UriKind.RelativeOrAbsolute);
            var requestUri = this.GetRequestUri(baseUri, relative);

            context.RequestMessage.Method = this.Method;
            context.RequestMessage.RequestUri = requestUri;
            return ApiTask.CompletedTask;
        }

        /// <summary>
        /// 获取请求URL
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="relative"></param>
        /// <exception cref="HttpApiConfigException"></exception>
        /// <returns></returns>
        private Uri GetRequestUri(Uri baseUri, Uri relative)
        {
            if (baseUri == null)
            {
                if (relative == null || relative.IsAbsoluteUri == true)
                {
                    return relative;
                }
                throw new HttpApiConfigException($"未配置HttpHost，无法应用路径{relative}");
            }
            else
            {
                if (relative == null)
                {
                    return baseUri;
                }
                return new Uri(baseUri, relative);
            }
        }
    }
}
