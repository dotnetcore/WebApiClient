using System;
using System.Net.Http;

namespace WebApiClient
{
    /// <summary>
    /// 表示Http请求重试异常
    /// </summary>
    public class RetryException : HttpRequestException
    {
        /// <summary>
        /// 重试异常
        /// </summary>
        /// <param name="message">提示</param>
        public RetryException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 重试异常
        /// </summary>
        /// <param name="message">提示</param>
        /// <param name="inner">内部异常</param>
        public RetryException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
