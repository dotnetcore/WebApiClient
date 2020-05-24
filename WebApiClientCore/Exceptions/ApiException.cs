using System;

namespace WebApiClientCore.Exceptions
{
    /// <summary>
    /// 表示接口异常基础抽象类
    /// </summary>
    public abstract class ApiException : Exception
    {
        /// <summary>
        /// 接口异常基础类
        /// </summary>
        public ApiException()
            : base()
        {
        }

        /// <summary>
        /// 接口异常基础类
        /// </summary>
        /// <param name="message">异常消息</param>
        public ApiException(string? message)
            : base(message)
        {
        }

        /// <summary>
        /// 接口异常基础类
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="inner">内部异常</param>
        public ApiException(string? message, Exception? inner)
            : base(message, inner)
        {
        }
    }
}
