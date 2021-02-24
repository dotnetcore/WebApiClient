using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 表示默认的token提供者工厂
    /// </summary>
    sealed class TokenProviderFactory : ITokenProviderFactory
    {
        private readonly IServiceProvider serviceProvider;
        private readonly TokenProviderFactoryOptions options;
        private readonly ConcurrentDictionary<Type, ITokenProvider> tokenProviderCache = new ConcurrentDictionary<Type, ITokenProvider>();

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
                var service = this.serviceProvider.GetRequiredService(serviceType);
                return ((ITokenProviderService)service).TokenProvider;
            }

            if (typeMatchMode == TypeMatchMode.TypeOrBaseTypes)
            {
                return this.tokenProviderCache.GetOrAdd(httpApiType, GetTokenProviderFromBaseType);
            }

            throw new InvalidOperationException($"尚未注册{httpApiType}的token提供者");
        }

        /// <summary>
        /// 从基础接口获取TokenProvider
        /// </summary>
        /// <param name="httpApiType"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        private ITokenProvider GetTokenProviderFromBaseType(Type httpApiType)
        {
            foreach (var baseType in httpApiType.GetInterfaces())
            {
                if (this.options.TryGetValue(baseType, out var serviceType))
                {
                    var service = this.serviceProvider.GetRequiredService(serviceType);
                    return ((ITokenProviderService)service).TokenProvider;
                }
            }
            throw new InvalidOperationException($"尚未注册{httpApiType}或其基础接口的token提供者");
        }
    }
}
