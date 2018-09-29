namespace WebApiClient
{
    /// <summary>
    /// 定义HttpApi工厂的接口
    /// </summary>
    public interface IHttpApiFactory
    {
        /// <summary>
        /// 创建接口的代理实例
        /// </summary>
        /// <returns></returns>
        object CreateHttpApi();
    }

    /// <summary>
    /// 定义HttpApi工厂的接口
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    public interface IHttpApiFactory<TInterface> : IHttpApiFactory where TInterface : class, IHttpApi
    {
        /// <summary>
        /// 创建接口的代理实例
        /// </summary>
        /// <returns></returns>
        new TInterface CreateHttpApi();
    }
}
