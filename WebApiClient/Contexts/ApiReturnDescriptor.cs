using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebApiClient.Attributes;
using WebApiClient.Attributes.ReturnAttributes;

namespace WebApiClient.Contexts
{
    /// <summary>
    /// 表示请求Api的返回描述
    /// </summary>
    [DebuggerDisplay("ReturnType = {ReturnType}")]
    public class ApiReturnDescriptor
    {
        /// <summary>
        /// 获取关联的ApiReturnAttribute
        /// </summary>
        public IApiReturnAttribute Attribute { get; protected set; }

        /// <summary>
        /// 获取返回类型
        /// </summary>
        public Type ReturnType { get; protected set; }

        /// <summary>
        /// 获取返回类型是否为定义为Task(Of T)
        /// </summary>
        public bool IsTaskDefinition { get; protected set; }

        /// <summary>
        /// 获取返回类型是否为定义为ITask(Of T)
        /// </summary>
        public bool IsITaskDefinition { get; protected set; }

        /// <summary>
        /// 获取ITask(Of T)或Task(Of T)的T类型描述
        /// </summary>
        public Type ReturnDataType { get; protected set; }

        /// <summary>
        /// 获取ITask(Of T)或Task(Of T)的T类型描述
        /// </summary>
        public DataTypeDescriptor DataType { get; protected set; }

        /// <summary>
        /// 获取返回值Mapper
        /// </summary>
        public IReturnValueMapper ReturnValueMapper { get; protected set; }

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

            var returnAttribute = method.FindDeclaringAttribute<IApiReturnAttribute>(true);
            if (returnAttribute == null)
            {
                returnAttribute = new AutoReturnAttribute();
            }

            var dataTypeDefinition = method.ReturnType.GetGenericTypeDefinition();

            this.Attribute = returnAttribute;
            this.ReturnType = method.ReturnType;
            this.ReturnDataType = method.ReturnType.GetGenericArguments().FirstOrDefault();
            this.IsTaskDefinition = dataTypeDefinition == typeof(Task<>);
            this.IsITaskDefinition = dataTypeDefinition == typeof(ITask<>);

            var returnValueMapperAttribute = method.FindDeclaringAttribute<ReturnValueMapperAttribute>(true);
            if (returnValueMapperAttribute != null)
            {
                ReturnValueMapper = (IReturnValueMapper)Activator.CreateInstance(returnValueMapperAttribute.ReturnValueMapperType);

                this.DataType = new DataTypeDescriptor(returnValueMapperAttribute.ResponseType);
            }
            else
            {
                this.DataType = new DataTypeDescriptor(ReturnDataType);
            }
        }
    }
}
