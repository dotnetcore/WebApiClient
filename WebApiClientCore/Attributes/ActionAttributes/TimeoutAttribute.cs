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
    [DebuggerDisplay("Timeout = {TimeSpan}")]
    public partial class TimeoutAttribute : ApiActionAttribute
    {
        /// <summary>
        /// 获取超时时间
        /// </summary>
        public TimeSpan? TimeSpan { get; }

        /// <summary>
        /// 指定请求的超时时间
        /// </summary>
        /// <param name="milliseconds">超时时间的毫秒数</param>
        [AttributeCtorUsage(AttributeTargets.Interface | AttributeTargets.Method)]
        public TimeoutAttribute(int milliseconds)
            : this((double)milliseconds)
        {
        }

        /// <summary>
        /// 指定请求的超时时间
        /// </summary>
        /// <param name="milliseconds">超时时间的毫秒数</param>
        [AttributeCtorUsage(AttributeTargets.Interface | AttributeTargets.Method)]
        public TimeoutAttribute(double milliseconds)
        {
            this.TimeSpan = System.TimeSpan.FromMilliseconds(milliseconds);
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <exception cref="ApiInvalidConfigException"></exception>
        /// <returns></returns>
        public override Task OnRequestAsync(ApiRequestContext context)
        {
            if (this.TimeSpan == null)
            {
                throw new ApiInvalidConfigException(Resx.parameter_CannotMissing.Format("milliseconds"));
            }

            SetTimeout(context, this.TimeSpan.Value);
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
            if (maxTimeout >= System.TimeSpan.Zero && timeout > maxTimeout)
            {
                throw new ApiInvalidConfigException(Resx.timeout_OutOfRange.Format(timeout));
            }

            var cancellation = new CancellationTokenSource(timeout);
            context.HttpContext.CancellationTokens.Add(cancellation.Token);
        }
    }
}
