using System;
using WebApiClientCore;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// token提供者创建器
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    class DefaultTokenProviderBuilder<THttpApi> : ITokenProviderBuilder
    {
        /// <summary>
        /// 获取别名
        /// </summary>
        public string Name { get; } = HttpApi.GetName<THttpApi>();

        /// <summary>
        /// 获取接口类型
        /// </summary>
        public Type HttpApiType { get; } = typeof(THttpApi);

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
