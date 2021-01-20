using System;
using System.Reflection;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义Action提供者的接口
    /// </summary>
    public interface IApiActionProvider
    {
        /// <summary>
        /// 创建Action描述
        /// </summary>
        /// <param name="method">接口的方法</param>
        /// <param name="interfaceType">接口类型</param> 
        ApiActionDescriptor CreateDescriptor(MethodInfo method, Type interfaceType);

        /// <summary>
        /// 创建Action执行器
        /// </summary>
        /// <param name="apiAction">Action描述</param>
        /// <returns></returns>
        IActionInvoker CreateInvoker(ApiActionDescriptor apiAction);
    }
}
