using System;

namespace WebApiClientCore.Exceptions
{
    /// <summary>
    /// 表示Api请求重试异常
    /// </summary>
    public class ApiRetryException : HttpApiException
    {
        /// <summary>
        /// 获取重试的最大次数
        /// </summary>
        public int MaxRetryCount { get; }

        /// <summary>
        /// Api请求重试异常
        /// </summary>
        /// <param name="maxRetryCount">重试的最大次数</param>   
        /// <param name="inner">内部异常</param>
        public ApiRetryException(int maxRetryCount, Exception? inner)
            : base(Resx.outof_MaxLimited.Format(maxRetryCount), inner)
        {
            this.MaxRetryCount = maxRetryCount;
        }
    }
}
