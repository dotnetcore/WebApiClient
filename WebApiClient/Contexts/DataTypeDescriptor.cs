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
        /// 表示空集合类型
        /// </summary>
        private static readonly Type[] emptyTypes = new Type[0];

        /// <summary>
        /// 获取类型
        /// </summary>
        public Type Type { get; protected set; }

        /// <summary>
        /// 获取是否为HttpResponseWrapper子类型
        /// </summary>
        public bool IsHttpResponseWrapper { get; protected set; }

        /// <summary>
        /// 获取包装为ITask的创建工厂
        /// </summary>
        public Func<ITask> ITaskFactory { get; protected set; }

        /// <summary>
        /// 获取包装为ITask的泛型构造器
        /// </summary>
        public ConstructorInfo ITaskConstructor { get; protected set; }

        /// <summary>
        /// 返回的Task(Of T)的T类型描述
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        public DataTypeDescriptor(Type dataType)
        {
            if (dataType == null)
            {
                throw new ArgumentNullException(nameof(dataType));
            }

            var taskType = typeof(ApiTask<>).MakeGenericType(dataType);

            this.Type = dataType;
            this.ITaskFactory = Lambda.CreateNewFunc<ITask>(taskType);
            this.ITaskConstructor = taskType.GetConstructor(emptyTypes);
            this.IsHttpResponseWrapper = dataType.IsInheritFrom<HttpResponseWrapper>();
        }
    }
}
