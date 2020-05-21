using System;

namespace WebApiClientCore.Exceptions
{
    /// <summary>
    /// 表示结果不匹配异常
    /// </summary>
    public class ResultNotMatchException : Exception
    {
        /// <summary>
        /// 获取结果值
        /// </summary>
        public object? Result { get; }

        /// <summary>
        /// 结果不匹配异常
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="result">结果值</param>
        public ResultNotMatchException(string message, object? result)
            : base(message)
        {
            this.Result = result;
        }
    }
}
