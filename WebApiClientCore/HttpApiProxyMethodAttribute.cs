using System;
using System.ComponentModel;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示HttpApi代理方法的特性 
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class HttpApiProxyMethodAttribute : Attribute
    {
        /// <summary>
        /// 获取索引值
        /// </summary>
        public int Index { get; } = -1;

        /// <summary>
        /// 获取名称
        /// </summary>
        public string Name { get; } = string.Empty;

        /// <summary>
        /// 获取方法所在的声明类型
        /// </summary>
        public Type? DeclaringType { get; }

        /// <summary>
        /// 方法的索引特性
        /// </summary>
        /// <param name="index">索引值</param>
        [Obsolete("仅为了兼容v2.0.8的SourceGenerator语法")]
        public HttpApiProxyMethodAttribute(int index)
        {
        }

        /// <summary>
        /// 方法的索引特性
        /// </summary>
        /// <param name="index">索引值</param>
        /// <param name="name">方法的名称</param>
        [Obsolete("仅为了兼容v2.0.9的SourceGenerator语法")]
        public HttpApiProxyMethodAttribute(int index, string name)
        {
        }

        /// <summary>
        /// 方法的索引特性
        /// </summary>
        /// <param name="index">索引值</param>
        /// <param name="name">方法的名称</param>
        /// <param name="declaringType">法所在的声明类型</param>
        public HttpApiProxyMethodAttribute(int index, string name, Type? declaringType)
        {
            this.Index = index;
            this.Name = name;
            this.DeclaringType = declaringType;
        }
    }
}
