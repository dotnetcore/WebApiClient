using System;

namespace WebApiClient
{
    /// <summary>
    /// 表示重试异常
    /// </summary>
    public class RetryException : Exception
    {
        /// <summary>
        /// 重试异常
        /// </summary>
        /// <param name="message">提示</param>
        public RetryException(string message)
            : base(message)
        {
        }
    }
}
