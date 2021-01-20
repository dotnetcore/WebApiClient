using System;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示此请求的超时时间
    /// </summary> 
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public partial class TimeoutAttribute : IApiParameterAttribute
    {
        /// <summary>
        /// 表示将参数值作为请求的超时时间
        /// 支持参数类型为数值类型和TimeSpan类型，以及他们的可空类型
        /// </summary>
        [AttributeCtorUsage(AttributeTargets.Parameter)]
        public TimeoutAttribute()
        {
        }

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param> 
        /// <exception cref="ApiInvalidConfigException"></exception>
        /// <returns></returns>
        public Task OnRequestAsync(ApiParameterContext context)
        {
            var timeout = context.ParameterValue;
            if (timeout != null)
            {
                var timespan = ConvertToTimeSpan(timeout);
                SetTimeout(context, timespan);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 将参数值转换为TimeSpan
        /// </summary>
        /// <param name="timeout"></param>
        /// <exception cref="ApiInvalidConfigException"></exception>
        /// <returns></returns>
        private static TimeSpan ConvertToTimeSpan(object timeout)
        {
            if (timeout is TimeSpan timespan)
            {
                return timespan;
            }

            if (timeout is IConvertible convertible)
            {
                var milliseconds = convertible.ToDouble(null);
                return System.TimeSpan.FromMilliseconds(milliseconds);
            }

            throw new ApiInvalidConfigException(Resx.parameter_CannotCvtTimeout.Format(timeout));
        }
    }
}
