using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        /// 非200响应时的缓存毫秒数（默认3秒，设为0则不缓存）
        /// </summary>
        public double ErrorExpiration { get; set; }

        /// <summary>
        /// 是否允许对非 GET 请求进行缓存（默认为 false，需显式二次确认）
        /// </summary>
        public bool EnableNonGet { get; set; }

        /// <summary>
        /// 用于在运行时存储动态计算的过期时间
        /// </summary>
        private TimeSpan? _dynamicExpiration;

        /// <summary>
        /// 重写 Expiration 属性，使其能返回动态调整后的值
        /// </summary>
        public new TimeSpan Expiration => _dynamicExpiration ?? base.Expiration;

        /// <summary>
        /// 缓存键的请求头名称
        /// </summary>
        private string? includeHeaderString;

        /// <summary>
        /// 获取缓存键的请求头名称
        /// </summary>
        protected string[] IncludeHeaderNames { get; private set; } = [];

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
        /// <param name="errorExpiration">接口异常时 缓存毫秒数，默认：3秒</param>
        /// <param name="eableNonGet">是否允许对非 GET 请求进行缓存</param>
        public CacheAttribute(double expiration, double errorExpiration = 3000, bool eableNonGet = false)
            : base(expiration)
        {
            this.ErrorExpiration = errorExpiration;
            this.EnableNonGet = eableNonGet;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override CachePolicy GetWritePolicy(ApiRequestContext context)
        {
            var request = context.HttpContext.RequestMessage;
            var response = context.HttpContext.ResponseMessage;

            // 判断是否为非 GET 方法
            if (request.Method != HttpMethod.Get)
            {
                if (!this.EnableNonGet)
                {
                    // 输出请求路径和方法，提醒开发者该非GET请求已被跳过缓存
                    Console.WriteLine($"[WARN] Cache Ignored: Method {request.Method} on {request.RequestUri} requires 'EnableNonGet=true'.");
                    return CachePolicy.Ignore;
                }
                else
                {
                    // 表示这是特殊处理的非GET缓存
                    Console.WriteLine($"[INFO] Cache Enabled for Non-GET: {request.RequestUri}");
                }
            }

            // 接口升级/异常状态（非 200）
            if (response != null && response.StatusCode != HttpStatusCode.OK)
            {
                if (this.ErrorExpiration <= 0)
                {
                    return CachePolicy.Ignore;
                }

                // 动态记录短缓存时间
                _dynamicExpiration = TimeSpan.FromMilliseconds(this.ErrorExpiration);
                return CachePolicy.Include;
            }

            // 200 OK 正常情况
            _dynamicExpiration = null; // 恢复使用 base.Expiration
            return CachePolicy.Include;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override CachePolicy GetReadPolicy(ApiRequestContext context)
        {
            var request = context.HttpContext.RequestMessage;
            // 如果不是 GET 且没有明确开启非GET支持
            if (request.Method != HttpMethod.Get && !this.EnableNonGet)
            {
                // 跳过之前的缓存
                Console.WriteLine($"[WARN] Cache Ignored: Method {request.Method} on {request.RequestUri} requires 'EnableNonGet=true'.");
                return CachePolicy.Ignore;
            }
            return CachePolicy.Include;
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
                    .Split([',', '|', ';'])
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
