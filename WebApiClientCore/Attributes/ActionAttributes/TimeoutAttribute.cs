using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示此请求的超时时间
    /// </summary>
    [DebuggerDisplay("Timeout = {Timeout}")]
    public partial class TimeoutAttribute : ApiActionAttribute
    {
        /// <summary>
        /// 获取超时时间
        /// </summary>
        public TimeSpan Timeout { get; } = TimeSpan.Zero;

        /// <summary>
        /// 指定请求的超时时间
        /// </summary>
        /// <param name="milliseconds">超时时间的毫秒数</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [AttributeCtorUsage(AttributeTargets.Interface | AttributeTargets.Method)]
        public TimeoutAttribute(int milliseconds)
            : this((double)milliseconds)
        {
        }

        /// <summary>
        /// 指定请求的超时时间
        /// </summary>
        /// <param name="milliseconds">超时时间的毫秒数</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [AttributeCtorUsage(AttributeTargets.Interface | AttributeTargets.Method)]
        public TimeoutAttribute(double milliseconds)
        {
            if (milliseconds <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(milliseconds));
            }
            this.Timeout = TimeSpan.FromMilliseconds(milliseconds);
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <exception cref="ApiInvalidConfigException"></exception>
        /// <returns></returns>
        public override Task OnRequestAsync(ApiRequestContext context)
        {
            SetTimeout(context, this.Timeout);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 设置超时时间到上下文
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="timeout">超时时间</param>
        /// <exception cref="ApiInvalidConfigException"></exception>
        private static void SetTimeout(ApiRequestContext context, TimeSpan timeout)
        {
            var maxTimeout = context.HttpContext.HttpClient.Timeout;
            if (maxTimeout >= TimeSpan.Zero && timeout > maxTimeout)
            {
                throw new ApiInvalidConfigException(Resx.timeout_OutOfRange.Format(timeout));
            }

            var cancellation = new CancellationTokenSource(timeout);
            context.HttpContext.CancellationTokens.Add(cancellation.Token);
        }
    }
}
