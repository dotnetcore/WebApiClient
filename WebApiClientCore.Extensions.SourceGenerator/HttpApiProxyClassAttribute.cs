using System;
using System.ComponentModel;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示HttpApi代理类的特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class HttpApiProxyClassAttribute : Attribute
    {
        /// <summary>
        /// 获取所代理的目标接口
        /// </summary>
        public Type HttpApiType { get; }

        /// <summary>
        /// 代理类型的特性
        /// </summary>
        /// <param name="httpApiType">所代理的目标接口</param>
        public HttpApiProxyClassAttribute(Type httpApiType)
        {
            this.HttpApiType = httpApiType;
        }
    }
}
