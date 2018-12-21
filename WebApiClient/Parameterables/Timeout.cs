using System;
using System.Diagnostics;
using System.Threading;
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
        public Timeout(int milliseconds)
            : this((double)milliseconds)
        {
        }

        /// <summary>
        /// 请求的超时时间
        /// </summary>
        /// <param name="milliseconds">超时时间的毫秒数</param>
        public Timeout(double milliseconds)
            : this(TimeSpan.FromMilliseconds(milliseconds))
        {
        }

        /// <summary>
        /// 请求的超时时间
        /// </summary>
        /// <param name="timeSpan">超时时间</param>
        public Timeout(TimeSpan timeSpan)
        {
            this.TimeSpan = timeSpan;
        }

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <exception cref="HttpApiConfigException"></exception>
        /// <returns></returns>
        public Task BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
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
