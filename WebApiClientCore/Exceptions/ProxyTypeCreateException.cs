using System;

namespace WebApiClientCore.Exceptions
{
    /// <summary>
    /// 表示代理类创建异常
    /// </summary>
    public class ProxyTypeCreateException : ProxyTypeException
    {
        /// <summary>
        /// 代理类创建异常
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        public ProxyTypeCreateException(Type interfaceType)
            : base(interfaceType)
        {
        }

        /// <summary>
        /// 代理类创建异常
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <param name="message">提示消息</param>
        public ProxyTypeCreateException(Type interfaceType, string? message)
            : base(interfaceType, message)
        {
        }
    }
}
