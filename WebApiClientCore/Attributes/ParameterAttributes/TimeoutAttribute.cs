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
            if (context.ParameterValue == null)
            {
                return Task.CompletedTask;
            }

            if (context.ParameterValue is TimeSpan timespan)
            {
                SetTimeout(context, timespan);
            }
            else if (context.ParameterValue is IConvertible convertible)
            {
                var milliseconds = convertible.ToDouble(null);
                var timeout = System.TimeSpan.FromMilliseconds(milliseconds);
                SetTimeout(context, timeout);
            }
            else
            {
                throw new ApiInvalidConfigException(Resx.parameter_CannotCvtTimeout.Format(context.Parameter.Member));
            }

            return Task.CompletedTask;
        }
    }
}
