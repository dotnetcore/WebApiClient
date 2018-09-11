using System;
using System.Diagnostics;

namespace WebApiClient.Contexts
{
    /// <summary>
    /// 表示请求Api的返回描述
    /// </summary>
    [DebuggerDisplay("ReturnType = {ReturnType.Type}")]
    public class ApiReturnDescriptor
    {
        /// <summary>
        /// 获取关联的ApiReturnAttribute
        /// </summary>
        public IApiReturnAttribute Attribute { get; internal set; }

        /// <summary>
        /// 获取返回类型
        /// </summary>
        public Type ReturnType { get; internal set; }

        /// <summary>
        /// 获取返回类型是否为定义为Task(Of T)
        /// </summary>
        public bool IsTaskDefinition { get; internal set; }

        /// <summary>
        /// 获取返回类型是否为定义为ITask(Of T)
        /// </summary>
        public bool IsITaskDefinition { get; internal set; }

        /// <summary>
        /// 获取ITask(Of T)或Task(Of T)的T类型描述
        /// </summary>
        public DataTypeDescriptor DataType { get; internal set; }
    }
}
