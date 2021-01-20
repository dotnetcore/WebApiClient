using System;
using System.Collections.Generic;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示请求Api的返回描述
    /// </summary>
    public abstract class ApiReturnDescriptor
    {
        /// <summary>
        /// 获取返回类型
        /// </summary>
        public abstract Type ReturnType { get; protected set; }

        /// <summary>
        /// 获取ITask(Of T)或Task(Of T)的T类型描述
        /// </summary>
        public abstract ApiDataTypeDescriptor DataType { get; protected set; }

        /// <summary>
        /// 获取关联的IApiReturnAttribute
        /// </summary>
        public abstract IReadOnlyList<IApiReturnAttribute> Attributes { get; protected set; }
    }
}
