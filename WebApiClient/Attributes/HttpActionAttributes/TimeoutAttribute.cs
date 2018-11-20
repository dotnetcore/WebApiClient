using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示此请求的超时时间
    /// </summary>
    [DebuggerDisplay("Timeout = {TimeSpan}")]
    public class TimeoutAttribute : ApiActionAttribute
    {
        /// <summary>
        /// 获取超时时间
        /// </summary>
        public TimeSpan TimeSpan { get; private set; }

        /// <summary>
        /// 请求的超时时间
        /// </summary>
        /// <param name="milliseconds">超时时间的毫秒数</param>
        public TimeoutAttribute(double milliseconds)
        {
            this.TimeSpan = TimeSpan.FromMilliseconds(milliseconds);
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <exception cref="HttpApiConfigException"></exception>
        /// <returns></returns>
        public override Task BeforeRequestAsync(ApiActionContext context)
        {
            var maxTimeout = context.HttpApiConfig.HttpClient.Timeout;
            if (maxTimeout >= TimeSpan.Zero && this.TimeSpan > maxTimeout)
            {
                throw new HttpApiConfigException($"Timeout值{this.TimeSpan}不能超时HttpApiConfig.HttpClient.Timeout");
            }

            var cancellation = new CancellationTokenSource(this.TimeSpan);
            context.CancellationTokens.Add(cancellation.Token);
            return ApiTask.CompletedTask;
        }
    }
}
