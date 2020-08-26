using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示使用缓存的特性
    /// 使用请求Uri作请求结果的缓存键
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CacheAttribute : ApiCacheAttribute
    {
        /// <summary>
        /// 缓存键的请求头名称
        /// </summary>
        private string? includeHeaderString;

        /// <summary>
        /// 获取缓存键的请求头名称
        /// </summary>
        protected string[] IncludeHeaderNames { get; private set; } = Array.Empty<string>();

        /// <summary>
        /// 获取或设置连同作为缓存键的请求头名称
        /// 多个请求头使用英文逗号分隔
        /// </summary>
        public string? IncludeHeaders
        {
            get => this.includeHeaderString;
            set => this.SetIncludeHeaders(value);
        }

        /// <summary>
        /// 使用缓存的特性
        /// </summary>
        /// <param name="expiration">缓存毫秒数</param>
        public CacheAttribute(double expiration)
            : base(expiration)
        {
        }

        /// <summary>
        /// 使用缓存的特性
        /// </summary>
        /// <param name="expiration"></param>
        protected CacheAttribute(TimeSpan expiration)
            : base(expiration)
        {
        }

        /// <summary>
        /// 设置作为缓存键的请求头
        /// </summary>
        /// <param name="headersString"></param>
        private void SetIncludeHeaders(string? headersString)
        {
            this.includeHeaderString = headersString;
            if (headersString != null)
            {
                this.IncludeHeaderNames = headersString
                    .Split(new[] { ',', '|', ';' })
                    .Select(h => h.Trim())
                    .Where(h => string.IsNullOrEmpty(h) == false)
                    .ToArray();
            }
        }

        /// <summary>
        /// 返回缓存的键
        /// 该键用于读取或写入缓存到缓存提供者
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override Task<string> GetCacheKeyAsync(ApiRequestContext context)
        {
            var request = context.HttpContext.RequestMessage;
            if (request.RequestUri == null)
            {
                throw new ApiInvalidConfigException(Resx.required_RequestUri);
            }

            var uri = request.RequestUri.ToString();
            if (this.IncludeHeaderNames.Length == 0)
            {
                return Task.FromResult(uri);
            }

            var builder = new StringBuilder(uri);
            foreach (var name in this.IncludeHeaderNames)
            {
                var value = string.Empty;
                if (request.Headers.TryGetValues(name, out IEnumerable<string>? values))
                {
                    value = string.Join(",", values);
                }
                builder.Append($"{name}:{value}");
            }

            return Task.FromResult(builder.ToString());
        }
    }
}
