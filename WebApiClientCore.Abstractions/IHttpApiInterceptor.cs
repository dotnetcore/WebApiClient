namespace WebApiClientCore
{
    /// <summary>
    /// 接口方法的拦截器
    /// </summary>
    public interface IHttpApiInterceptor
    {
        /// <summary>
        /// 拦截方法的调用
        /// </summary>
        /// <param name="actionInvoker">action执行器</param> 
        /// <param name="arguments">action的参数集合</param>
        /// <returns></returns>
        object Intercept(ApiActionInvoker actionInvoker, object?[] arguments);
    }
}
