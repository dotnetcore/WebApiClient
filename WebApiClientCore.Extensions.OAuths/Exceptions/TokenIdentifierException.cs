using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiClientCore.Extensions.OAuths.Exceptions
{
    /// <summary>
    /// 表示Token 应用标识异常
    /// </summary>
    public class TokenIdentifierException : Exception
    {
        /// <summary>
        /// Token异常
        /// </summary>
        /// <param name="message">消息</param>
        public TokenIdentifierException(string? message)
            : base(message)
        {
        }

        /// <summary>
        /// 表示Token 应用标识异常
        /// </summary>
        public TokenIdentifierException()
        {
        }

        /// <summary>
        /// 表示Token 应用标识异常
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
        public TokenIdentifierException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
