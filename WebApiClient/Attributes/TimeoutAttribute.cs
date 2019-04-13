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
        /// <exception cref="HttpApiConfigException"></exception>
        /// <returns></returns>
        public override Task BeforeRequestAsync(ApiActionContext context)
        {
            if (this.TimeSpan.HasValue == false)
            {
                throw new HttpApiConfigException($"请传入milliseconds参数：{nameof(TimeoutAttribute)}");
            }

            this.SetTimeout(context, this.TimeSpan.Value);
            return ApiTask.CompletedTask;
        }

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <exception cref="HttpApiConfigException"></exception>
        /// <returns></returns>
        async Task IApiParameterAttribute.BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            if (parameter.Value == null)
            {
                return;
            }

            if (parameter.Value is TimeSpan timespan)
            {
                this.SetTimeout(context, timespan);
            }
            else if (parameter.Value is IConvertible convertible)
            {
                var milliseconds = convertible.ToDouble(null);
                var timeout = System.TimeSpan.FromMilliseconds(milliseconds);
                this.SetTimeout(context, timeout);
            }
            else
            {
                throw new HttpApiConfigException($"无法将参数{parameter.Member}转换为Timeout");
            }
            await ApiTask.CompletedTask;
        }


        /// <summary>
        /// 设置超时时间到上下文
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="timeout">超时时间</param>
        /// <exception cref="HttpApiConfigException"></exception>
        private void SetTimeout(ApiActionContext context, TimeSpan timeout)
        {
            var maxTimeout = context.HttpApiConfig.HttpClient.Timeout;
            if (maxTimeout >= System.TimeSpan.Zero && timeout > maxTimeout)
            {
                throw new HttpApiConfigException($"Timeout值{timeout}不能超时HttpApiConfig.HttpClient.Timeout");
            }

            var cancellation = new CancellationTokenSource(timeout);
            context.CancellationTokens.Add(cancellation.Token);
        }
    }
}
