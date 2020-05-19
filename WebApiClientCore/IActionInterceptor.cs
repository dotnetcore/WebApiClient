using System;
using System.ComponentModel;
using System.Reflection;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义方法接口拦截器的行为
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IActionInterceptor
    {
        /// <summary>
        /// 拦截方法的调用
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <param name="method">接口的方法</param>
        /// <param name="arguments">方法的参数集合</param>
        /// <returns></returns>
        object Intercept(Type interfaceType, MethodInfo method, object[] arguments);
    }
}
