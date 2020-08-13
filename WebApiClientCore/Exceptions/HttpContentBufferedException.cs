using System;

namespace WebApiClientCore.Exceptions
{
    /// <summary>
    /// 表示HttpContent的数据已经缓存的异常
    /// </summary>
    public class HttpContentBufferedException : Exception
    {
        /// <summary>
        /// HttpContent的数据已经缓存的异常
        /// </summary>
        public HttpContentBufferedException()
            : this(Resx.httpContent_isBuffered)
        {
        }

        /// <summary>
        /// HttpContent的数据已经缓存的异常
        /// </summary>
        /// <param name="message">提示消息 </param>
        public HttpContentBufferedException(string message)
            : base(message)
        {
        }
    }
}
