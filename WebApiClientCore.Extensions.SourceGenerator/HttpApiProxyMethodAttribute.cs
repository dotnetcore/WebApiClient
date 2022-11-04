using System;
using System.ComponentModel;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示HttpApi代理方法的特性 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class HttpApiProxyMethodAttribute : Attribute
    {
        /// <summary>
        /// 获取索引值
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// 方法的索引特性
        /// </summary>
        /// <param name="index">索引值，确保连续且不重复</param>
        public HttpApiProxyMethodAttribute(int index)
        {
            this.Index = index;
        }
    }
}
