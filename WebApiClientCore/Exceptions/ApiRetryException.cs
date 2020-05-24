using System;

namespace WebApiClientCore.Exceptions
{
    /// <summary>
    /// 表示接口请求重试异常
    /// </summary>
    public class ApiRetryException : ApiException
    {
        /// <summary>
        /// 获取重试的最大次数
        /// </summary>
        public int MaxRetryCount { get; }

        /// <summary>
        /// 接口请求重试异常
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
