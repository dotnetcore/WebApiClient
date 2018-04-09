using System;
using System.Diagnostics;
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
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public TimeoutAttribute(int milliseconds)
        {
            if (milliseconds <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(milliseconds));
            }
            this.TimeSpan = TimeSpan.FromMilliseconds(milliseconds);
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override Task BeforeRequestAsync(ApiActionContext context)
        {
            context.RequestMessage.Timeout = this.TimeSpan;
            return ApiTask.CompletedTask;
        }
    }
}
