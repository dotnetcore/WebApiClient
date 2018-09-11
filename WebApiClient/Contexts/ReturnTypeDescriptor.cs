using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebApiClient.Contexts
{
    /// <summary>
    /// 表示返回类型描述
    /// </summary>
    public class ReturnTypeDescriptor
    {
        /// <summary>
        /// 获取返回类型
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// 获取ITask(Of T)或Task(Of T)的T类型描述
        /// </summary>
        public DataTypeDescriptor DataType { get; private set; }

        /// <summary>
        /// 获取返回类型是否为定义为Task(Of T)
        /// </summary>
        public bool IsTaskDefinition { get; private set; }

        /// <summary>
        /// 获取返回类型是否为定义为ITask(Of T)
        /// </summary>
        public bool IsITaskDefinition { get; private set; }

        /// <summary>
        /// 表示返回类型描述
        /// </summary>
        /// <param name="returnType">返回类型</param>
        public ReturnTypeDescriptor(Type returnType)
        {
            var dataType = returnType.GetGenericArguments().FirstOrDefault();
            var dataTypeDefinition = returnType.GetGenericTypeDefinition();

            this.Type = returnType;
            this.DataType = new DataTypeDescriptor(dataType);
            this.IsTaskDefinition = dataTypeDefinition == typeof(Task<>);
            this.IsITaskDefinition = dataTypeDefinition == typeof(ITask<>);
        }
    }
}
