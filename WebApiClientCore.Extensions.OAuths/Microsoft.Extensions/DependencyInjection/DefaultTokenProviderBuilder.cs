namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// token提供者创建器
    /// </summary>
    class DefaultTokenProviderBuilder : ITokenProviderBuilder
    {
        /// <summary>
        /// 获取token提供者的别名
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 获取服务描述集合
        /// </summary>
        public IServiceCollection Services { get; }

        /// <summary>
        /// token提供者创建器
        /// </summary>
        /// <param name="name">token提供者的别名</param>
        /// <param name="services">服务描述集合</param>
        public DefaultTokenProviderBuilder(string name, IServiceCollection services)
        {
            this.Name = name;
            this.Services = services;
        }
    }
}
