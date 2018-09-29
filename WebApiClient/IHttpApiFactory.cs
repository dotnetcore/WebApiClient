namespace WebApiClient
{
    /// <summary>
    /// 定义HttpApi工厂的接口
    /// </summary>
    public interface IHttpApiFactory
    {
        /// <summary>
        /// 创建指定接口的代理实例
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <returns></returns>
        TInterface CreateHttpApi<TInterface>() where TInterface : class, IHttpApi;
    }
}
