using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 表示默认的token提供者工厂
    /// </summary>
    sealed class TokenProviderFactory : ITokenProviderFactory
    {
        private readonly IServiceProvider serviceProvider;
        private readonly TokenProviderFactoryOptions options;

        /// <summary>
        /// 默认的token提供者工厂
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="options"></param>
        public TokenProviderFactory(IServiceProvider serviceProvider, IOptions<TokenProviderFactoryOptions> options)
        {
            this.serviceProvider = serviceProvider;
            this.options = options.Value;
        }

        /// <summary>
        /// 通过接口类型获取或创建其对应的token提供者
        /// </summary>
        /// <param name="httpApiType">接口类型</param>
        /// <param name="typeMatchMode">类型匹配模式</param>     
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public ITokenProvider Create(Type httpApiType, TypeMatchMode typeMatchMode)
        {
            if (httpApiType == null)
            {
                throw new ArgumentNullException(nameof(httpApiType));
            }

            if (this.options.TryGetValue(httpApiType, out var serviceType))
            {
                return this.GetTokenProvider(serviceType);
            }

            if (typeMatchMode == TypeMatchMode.TypeOrBaseTypes)
            {
                foreach (var baseType in httpApiType.GetInterfaces())
                {
                    if (this.options.TryGetValue(baseType, out serviceType))
                    {
                        return this.GetTokenProvider(serviceType);
                    }
                }
            }

            throw new InvalidOperationException($"尚未注册{httpApiType}的token提供者");
        }

        /// <summary>
        /// 从HttpApiTokenProviderService类型获取服务提供者
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        private ITokenProvider GetTokenProvider(Type serviceType)
        {
            var service = (ITokenProviderService)this.serviceProvider.GetRequiredService(serviceType);
            return service.TokenProvider;
        }
    }
}
