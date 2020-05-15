using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示请求Api的返回描述
    /// </summary>
    [DebuggerDisplay("ReturnType = {ReturnType}")]
    public class ApiReturnDescriptor
    {
        /// <summary>
        /// 获取返回类型
        /// </summary>
        public Type ReturnType { get; protected set; }

        /// <summary>
        /// 获取ITask(Of T)或Task(Of T)的T类型描述
        /// </summary>
        public ApiDataTypeDescriptor DataType { get; protected set; }

        /// <summary>
        /// 获取返回类型是否为Task定义
        /// </summary>
        public bool IsTaskDefinition { get; protected set; }

        /// <summary>
        /// 请求Api的返回描述
        /// </summary>
        /// <param name="method">方法信息</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiReturnDescriptor(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            var dataType = method.ReturnType.IsGenericType ?
                method.ReturnType.GetGenericArguments().FirstOrDefault() :
                typeof(HttpResponseMessage);

            this.ReturnType = method.ReturnType;
            this.DataType = new ApiDataTypeDescriptor(dataType);
            this.IsTaskDefinition = method.ReturnType.IsInheritFrom<Task>();
        }
    }
}
