#if NETSTANDARD2_1
namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>
    /// 表示动态依赖属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    sealed class DynamicDependencyAttribute : Attribute
    {
        /// <summary>
        /// 获取或设置动态访问的成员类型
        /// </summary>
        public DynamicallyAccessedMemberTypes MemberTypes { get; }

        /// <summary>
        /// 获取或设置依赖的类型
        /// </summary>
        public Type? Type { get; }

        /// <summary>
        /// 初始化 <see cref="DynamicDependencyAttribute"/> 类的新实例
        /// </summary>
        /// <param name="memberTypes">动态访问的成员类型</param>
        /// <param name="type">依赖的类型</param>
        public DynamicDependencyAttribute(DynamicallyAccessedMemberTypes memberTypes, Type type)
        {
            this.MemberTypes = memberTypes;
            this.Type = type;
        }
    }
}
#endif