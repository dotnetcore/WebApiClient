using System.ComponentModel;

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
        /// <param name="actionInvoker">action执行器</param> 
        /// <param name="arguments">方法的参数集合</param>
        /// <returns></returns>
        object Intercept(IActionInvoker actionInvoker, object[] arguments);
    }
}
