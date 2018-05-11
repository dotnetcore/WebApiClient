using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Parameterables
{
    /// <summary>
    /// 表示将自身作为请求的超时时间控制
    /// </summary>
    [DebuggerDisplay("Timeout = {TimeSpan}")]
    public class Timeout : IApiParameterable
    {
        /// <summary>
        /// 获取超时时间
        /// </summary>
        public TimeSpan TimeSpan { get; private set; }

        /// <summary>
        /// 请求的超时时间
        /// </summary>
        /// <param name="milliseconds">超时时间的毫秒数</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Timeout(int milliseconds)
        {
            if (milliseconds <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(milliseconds));
            }
            this.TimeSpan = TimeSpan.FromMilliseconds(milliseconds);
        }

        /// <summary>
        /// 请求的超时时间
        /// </summary>
        /// <param name="timeSpan">超时时间</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Timeout(TimeSpan timeSpan)
        {
            if (timeSpan <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(timeSpan));
            }
            this.TimeSpan = timeSpan;
        }

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        public Task BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            context.RequestMessage.Timeout = this.TimeSpan;
            return ApiTask.CompletedTask;
        }


        /// <summary>
        /// 从int类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Timeout(int value)
        {
            return new Timeout(value);
        }

        /// <summary>
        /// 从TimeSpan类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Timeout(TimeSpan value)
        {
            return new Timeout(value);
        }
    }
}
