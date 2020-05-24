using System;

namespace WebApiClientCore.Exceptions
{
    /// <summary>
    /// 表示类型实例创建异常
    /// </summary>
    public class TypeInstanceCreateException : Exception
    {
        /// <summary>
        /// 实例类型
        /// </summary>
        public Type InstanceType { get; }

        /// <summary>
        /// 类型实例创建异常
        /// </summary>
        /// <param name="instanceType">实例类型</param>
        public TypeInstanceCreateException(Type instanceType)
        {
            this.InstanceType = instanceType;
        }

    }
}
