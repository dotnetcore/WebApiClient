namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 表示接口方法的拦截器
    /// </summary>
    sealed class HttpApiInterceptor : IHttpApiInterceptor
    {
        /// <summary>
        /// 服务上下文
        /// </summary>
        private readonly HttpClientContext context;

        /// <summary>
        /// 接口方法的拦截器
        /// </summary>
        /// <param name="context">httpClient上下文</param> 
        public HttpApiInterceptor(HttpClientContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// 拦截方法的调用
        /// </summary>
        /// <param name="actionInvoker">action执行器</param> 
        /// <param name="arguments">方法的参数集合</param>
        /// <returns></returns>
        public object Intercept(ApiActionInvoker actionInvoker, object?[] arguments)
        {
            return actionInvoker.Invoke(this.context, arguments);
        }
    }
}
