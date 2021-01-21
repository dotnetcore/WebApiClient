namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// WebApiClient全局配置的Builder接口
    /// </summary>
    public interface IWebApiClientBuilder
    {
        /// <summary>
        /// 获取服务集合
        /// </summary>
        IServiceCollection Services { get; }
    }
}
