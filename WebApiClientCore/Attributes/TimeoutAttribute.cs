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
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class TimeoutAttribute : ApiActionAttribute, IApiParameterAttribute
    {
        /// <summary>
        /// 获取超时时间
        /// </summary>
        public TimeSpan? TimeSpan { get; }

        /// <summary>
        /// 表示将参数值作为请求的超时时间
        /// 支持参数类型为数值类型和TimeSpan类型，以及他们的可空类型
        /// </summary>
        [AttributeCtorUsage(AttributeTargets.Parameter)]
        public TimeoutAttribute()
        {
        }

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
        /// <exception cref="HttpApiInvalidOperationException"></exception>
        /// <returns></returns>
        public override Task OnRequestAsync(ApiRequestContext context)
        {
            if (this.TimeSpan.HasValue == false)
            {
                throw new HttpApiInvalidOperationException($"请传入milliseconds参数：{nameof(TimeoutAttribute)}");
            }

            this.SetTimeout(context, this.TimeSpan.Value);
            return Task.CompletedTask;
        }

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="next"></param>
        /// <exception cref="HttpApiInvalidOperationException"></exception>
        /// <returns></returns>
        public Task OnRequestAsync(ApiParameterContext context, Func<Task> next)
        {
            if (context.ParameterValue is TimeSpan timespan)
            {
                this.SetTimeout(context, timespan);
            }
            else if (context.ParameterValue is IConvertible convertible)
            {
                var milliseconds = convertible.ToDouble(null);
                var timeout = System.TimeSpan.FromMilliseconds(milliseconds);
                this.SetTimeout(context, timeout);
            }
            else
            {
                throw new HttpApiInvalidOperationException($"无法将参数{context.Parameter.Member}转换为Timeout");
            }

            return next();
        }


        /// <summary>
        /// 设置超时时间到上下文
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="timeout">超时时间</param>
        /// <exception cref="HttpApiInvalidOperationException"></exception>
        private void SetTimeout(ApiRequestContext context, TimeSpan timeout)
        {
            var maxTimeout = context.HttpContext.Client.Timeout;
            if (maxTimeout >= System.TimeSpan.Zero && timeout > maxTimeout)
            {
                throw new HttpApiInvalidOperationException(Resx.timeout_OutOfRange.Format(timeout));
            }

            var cancellation = new CancellationTokenSource(timeout);
            context.CancellationTokens.Add(cancellation.Token);
        }
    }
}
