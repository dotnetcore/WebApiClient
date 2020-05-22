using System;

namespace WebApiClientCore.Extensions.OAuths.Exceptions
{
    /// <summary>
    /// 表示Token异常
    /// </summary>
    public class TokenException : Exception
    {
        /// <summary>
        /// Token异常
        /// </summary>
        /// <param name="message">消息</param>
        public TokenException(string? message)
            : base(message)
        {
        }
    }
}
