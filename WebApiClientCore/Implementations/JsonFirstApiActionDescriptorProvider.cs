using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 当非GET或HEAD请求的缺省参数特性声明时
    /// 为复杂参数类型的参数应用JsonContentAttribute
    /// </summary>
    public class JsonFirstApiActionDescriptorProvider : IApiActionDescriptorProvider
    {
        /// <summary>
        /// 创建Action描述
        /// </summary>
        /// <param name="method">接口的方法</param>
        /// <param name="interfaceType">接口类型</param> 
        public ApiActionDescriptor CreateActionDescriptor(MethodInfo method,
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
            Type interfaceType)
        {
            return new JsonFirstApiActionDescriptor(method, interfaceType);
        }
    }
}
