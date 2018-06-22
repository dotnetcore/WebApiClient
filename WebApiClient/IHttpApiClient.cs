namespace WebApiClient
{
    /// <summary>
    /// 定义HttpApiClient的接口
    /// </summary>
    public interface IHttpApiClient : IHttpApi
    {
        /// <summary>
        /// 获取拦截器
        /// </summary>
        IApiInterceptor ApiInterceptor { get; }
    }
}
