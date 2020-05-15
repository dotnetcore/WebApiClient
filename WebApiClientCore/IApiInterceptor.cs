using System.ComponentModel;
using System.Reflection;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义http接口拦截器的行为
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IApiInterceptor
    {
        /// <summary>
        /// 拦截方法的调用
        /// </summary>
        /// <param name="target">接口的实例</param>
        /// <param name="method">接口的方法</param>
        /// <param name="arguments">接口的参数集合</param>
        /// <returns></returns>
        object Intercept(object target, MethodInfo method, object[] arguments);
    }
}
