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
        public int Index { get; }

        /// <summary>
        /// 获取方法所在的声明类型
        /// </summary>
        public Type? DeclaringType { get; }

        /// <summary>
        /// 获取名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 方法的索引特性
        /// </summary>
        /// <param name="index">索引值</param>
        /// <param name="declaringType">法所在的声明类型</param>
        /// <param name="name">方法的名称</param>
        public HttpApiProxyMethodAttribute(int index, Type? declaringType, string name)
        {
            this.Index = index;
            this.DeclaringType = declaringType;
            this.Name = name;
        }
    }
}
