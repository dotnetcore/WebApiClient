using System;
using System.Reflection;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义ApiActionDescriptor提供者的接口
    /// </summary>
    public interface IApiActionDescriptorProvider
    {
        /// <summary>
        /// 创建Action描述
        /// </summary>
        /// <param name="method">接口的方法</param>
        /// <param name="interfaceType">接口类型</param> 
        ApiActionDescriptor CreateActionDescriptor(MethodInfo method, Type interfaceType);
    }
}
