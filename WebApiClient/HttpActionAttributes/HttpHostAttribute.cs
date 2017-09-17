using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示请求服务根路径
    /// 不可继承
    /// </summary>
    public sealed class HttpHostAttribute : ApiActionAttribute
    {
        /// <summary>
        /// 获取根路径
        /// </summary>
        public Uri Host { get; private set; }

        /// <summary>
        /// 获取顺序排序索引
        /// 优先级最高
        /// </summary>
        public override int OrderIndex
        {
            get
            {
                return int.MinValue;
            }
        }

        /// <summary>
        /// 请求服务的根路径
        /// http://www.webapi.com/
        /// </summary>
        /// <param name="host">根路径</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        public HttpHostAttribute(string host)
        {
            this.Host = new Uri(host, UriKind.Absolute);
        }

        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override Task BeforeRequestAsync(ApiActionContext context)
        {
            context.RequestMessage.RequestUri = this.Host;
            return TaskExtend.CompletedTask;
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Host.ToString();
        }
    }
}
