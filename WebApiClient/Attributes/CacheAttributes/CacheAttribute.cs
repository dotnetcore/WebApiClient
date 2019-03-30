using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示使用缓存的特性
    /// 使用请求Uri作请求结果的缓存键
    /// 缓存功能依赖于HttpApiConfig.ResponseCacheProvider
    /// </summary>
    [DebuggerDisplay("Expiration = {Expiration}")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CacheAttribute : Attribute, IApiActionCacheAttribute
    {
        /// <summary>
        /// 缓存键的请求头名称
        /// </summary>
        private string includeHeaderString;

        /// <summary>
        /// 获取缓存键的请求头名称
        /// </summary>
        protected string[] IncludeHeaderNames { get; private set; } = new string[0];

        /// <summary>
        /// 获取缓存的时间戳
        /// </summary>
        public TimeSpan Expiration { get; }

        /// <summary>
        /// 获取或设置连同作为缓存键的请求头名称
        /// 多个请求头使用英文逗号分隔
        /// </summary>
        public string IncludeHeaders
        {
            get => this.includeHeaderString;
            set => this.SetHeaders(value);
        }

        /// <summary>
        /// 使用缓存的特性
        /// </summary>
        /// <param name="expiration">缓存毫秒数</param>
        public CacheAttribute(double expiration)
        {
            this.Expiration = TimeSpan.FromMilliseconds(expiration);
        }

        /// <summary>
        /// 设置请求头
        /// </summary>
        /// <param name="headersString"></param>
        private void SetHeaders(string headersString)
        {
            this.includeHeaderString = headersString;
            if (headersString != null)
            {
                this.IncludeHeaderNames = headersString
                    .Split(new[] { ',', '|' })
                    .Select(h => h.Trim())
                    .Where(h => string.IsNullOrEmpty(h) == false)
                    .ToArray();
            }
        }

        /// <summary>
        /// 返回缓存的键
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public virtual Task<string> GetCacheKeyAsync(ApiActionContext context)
        {
            var request = context.RequestMessage;
            var uri = request.RequestUri.ToString();
            var builder = new StringBuilder(uri);

            foreach (var name in this.IncludeHeaderNames)
            {
                var value = string.Empty;
                if (request.Headers.TryGetValues(name, out IEnumerable<string> values))
                {
                    value = string.Join(",", values);
                }
                builder.Append($"{name}:{value}");
            }

            return Task.FromResult(builder.ToString());
        }
    }
}
