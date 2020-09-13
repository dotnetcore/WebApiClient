namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// token提供者创建器接口
    /// </summary>
    public interface ITokenProviderBuilder
    {
        /// <summary>
        /// 获取token提供者所在的域
        /// </summary>
        string Domain { get; }

        /// <summary>
        /// 获取服务提供者
        /// </summary>
        IServiceCollection Services { get; }
    }
}
