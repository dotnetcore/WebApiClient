namespace WebApiClientCore
{
    /// <summary>
    /// 定义HttpApi激活器的接口
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    public interface IHttpApiActivator<THttpApi>
    {
        /// <summary>
        /// 创建THttpApi的代理实例
        /// </summary>
        /// <param name="interceptor">接口方法拦截器</param>
        /// <returns></returns>
        THttpApi CreateInstance(IActionInterceptor interceptor);
    }
}
