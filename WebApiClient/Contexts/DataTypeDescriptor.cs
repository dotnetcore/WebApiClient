using System;
using System.Diagnostics;
using System.Reflection;

namespace WebApiClient.Contexts
{
    /// <summary>
    /// 表示返回的Task(Of T)的T类型描述
    /// </summary>
    [DebuggerDisplay("Type = {Type}")]
    public class DataTypeDescriptor
    {
        /// <summary>
        /// 获取类型
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// 获取是否为HttpResponseWrapper子类型
        /// </summary>
        public bool IsHttpResponseWrapper { get; private set; }

        /// <summary>
        /// 获取包装为ITask的泛型构造器
        /// </summary>
        public ConstructorInfo ITaskConstructor { get; private set; }

        /// <summary>
        /// 返回的Task(Of T)的T类型描述
        /// </summary>
        /// <param name="dataType">数据类型</param>
        public DataTypeDescriptor(Type dataType)
        {
            this.Type = dataType;
            this.ITaskConstructor = ApiTask.GetITaskConstructor(dataType);
            this.IsHttpResponseWrapper = dataType.IsInheritFrom<HttpResponseWrapper>();
        }
    }
}
