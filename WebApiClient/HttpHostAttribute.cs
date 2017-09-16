using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示请求服务根路径
    /// 不可继承
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class HttpHostAttribute : Attribute
    {
        /// <summary>
        /// 获取根路径
        /// </summary>
        public Uri Host { get; private set; }

        /// <summary>
        /// 请求服务的根路径
        /// http://www.webapi.com/
        /// </summary>
        /// <param name="host">根路径</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        public HttpHostAttribute(string host)
        {
            this.Host = new Uri(host);
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
