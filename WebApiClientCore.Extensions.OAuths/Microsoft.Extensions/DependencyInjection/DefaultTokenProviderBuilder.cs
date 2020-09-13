using WebApiClientCore.Extensions.OAuths;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// token提供者创建器
    /// </summary>
    /// <typeparam name="TTokenProvider"></typeparam>
    class DefaultTokenProviderBuilder<TTokenProvider> : ITokenProviderBuilder where TTokenProvider : ITokenProvider
    {
        /// <summary>
        /// 获取token提供者名称
        /// </summary>
        public string Name { get; } = typeof(TTokenProvider).FullName;

        /// <summary>
        /// 获取服务描述集合
        /// </summary>
        public IServiceCollection Services { get; }

        /// <summary>
        /// token提供者创建器
        /// </summary>
        /// <param name="services">服务描述集合</param>
        public DefaultTokenProviderBuilder(IServiceCollection services)
        {
            this.Services = services;
        }
    }
}
