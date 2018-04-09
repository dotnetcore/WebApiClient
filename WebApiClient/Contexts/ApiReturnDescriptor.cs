using System;
using System.Diagnostics;
using System.Reflection;

namespace WebApiClient.Contexts
{
    /// <summary>
    /// 表示请求Api的返回描述
    /// </summary>
    [DebuggerDisplay("DataType = {DataType}")]
    public class ApiReturnDescriptor
    {
        /// <summary>
        /// 获取关联的ApiReturnAttribute
        /// </summary>
        public IApiReturnAttribute Attribute { get; internal set; }

        /// <summary>
        /// 获取Api的返回类型
        /// </summary>
        public Type ReturnType { get; internal set; }

        /// <summary>
        /// 获取Api返回的ITaskOf(T)或TaskOf(T)的T类型
        /// </summary>
        public Type DataType { get; internal set; }

        /// <summary>
        /// 获取返回类型是ITaskOf(T)而不是TaskOf(T)
        /// </summary>
        public bool IsITaskDefinition { get; internal set; }

        /// <summary>
        /// 获取返回类型对应的ITask泛型构造器
        /// 用于构造ITaskOf(T)的实例
        /// </summary>
        public ConstructorInfo ITaskCtor { get; internal set; }
    }
}
