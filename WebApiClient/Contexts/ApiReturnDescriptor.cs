using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace WebApiClient
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
        /// 获取Api返回的TaskOf(T)的T类型
        /// </summary>
        public Type DataType { get; internal set; }

        /// <summary>
        /// ITask返回类型对应的泛型构造器
        /// </summary>
        public ConstructorInfo ITaskCtor { get; internal set; }
    }
}
