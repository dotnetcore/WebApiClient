using System;

namespace WebApiClientCore
{
    /// <summary>
    /// 指示Api方法的索引特性
    /// 当生成代理类型使用该特性后，运行时HttpApi.FindApiMethods(Type)遵循这个方法索引
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ApiMethodIndexAttribute : Attribute
    {
        /// <summary>
        /// 获取索引值
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Api方法的索引特性
        /// </summary>
        /// <param name="index">索引值，确保连续且不重复</param>
        public ApiMethodIndexAttribute(int index)
        {
            this.Index = index;
        }
    }
}
