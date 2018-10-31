namespace WebApiClient
{
    /// <summary>
    /// 定义HttpApiFactory的接口
    /// </summary>
    interface IHttpApiFactory
    {
        /// <summary>
        /// 创建接口的代理实例
        /// </summary>
        /// <returns></returns>
        HttpApiClient CreateHttpApi();
    }
}
