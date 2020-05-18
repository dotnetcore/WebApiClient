using System;

namespace WebApiClientCore
{
    /// <summary> 
    /// 表示将参数别名 
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class AliasAsAttribute : Attribute
    {
        /// <summary>
        /// 获取别名
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 指定参数或属性的别名
        /// </summary>
        /// <param name="name">参数或属性的别名</param>
        /// <exception cref="ArgumentNullException"></exception>
        public AliasAsAttribute(string name)
        { 
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
        } 
    }
}
