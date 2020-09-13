using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 表示默认的token提供者工厂
    /// </summary>
    class TokenProviderFactory : ITokenProviderFactory
    {
        private readonly IServiceProvider serviceProvider;
        private readonly HttpApiTokenProviderMap httpApiTokenProviderMap;

        /// <summary>
        /// 默认的token提供者工厂
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="options"></param>
        public TokenProviderFactory(IServiceProvider serviceProvider, IOptions<HttpApiTokenProviderMap> options)
        {
            this.serviceProvider = serviceProvider;
            this.httpApiTokenProviderMap = options.Value;
        }

        /// <summary>
        /// 创建token提供者
        /// </summary>
        /// <param name="httpApiType">接口类型</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public ITokenProvider Create(Type httpApiType)
        {
            if (this.httpApiTokenProviderMap.TryGetValue(httpApiType, out var tokenProviderType))
            {
                return (ITokenProvider)this.serviceProvider.GetRequiredService(tokenProviderType);
            }
            throw new InvalidOperationException($"未注册{httpApiType}的token提供者");
        }
    }
}
