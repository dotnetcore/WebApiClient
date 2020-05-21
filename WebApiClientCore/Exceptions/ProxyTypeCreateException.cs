using System;

namespace WebApiClientCore.Exceptions
{
    /// <summary>
    /// 表示代理类创建异常
    /// </summary>
    public class ProxyTypeCreateException : Exception
    {
        /// <summary>
        /// 接口类型
        /// </summary>
        public Type InterfaceType { get; }

        /// <summary>
        /// 代理类创建异常
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        public ProxyTypeCreateException(Type interfaceType)
        {
            this.InterfaceType = interfaceType;
        }
    }
}
