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
        private readonly TokenProviderOptions options;

        /// <summary>
        /// 默认的token提供者工厂
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="options"></param>
        public TokenProviderFactory(IServiceProvider serviceProvider, IOptions<TokenProviderOptions> options)
        {
            this.serviceProvider = serviceProvider;
            this.options = options.Value;
        }

        /// <summary>
        /// 创建token提供者
        /// </summary>
        /// <param name="httpApiType">接口类型</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public ITokenProvider Create(Type httpApiType)
        {
            if (this.options.TryGetValue(httpApiType, out var domain))
            {
                return domain.CreateTokenProvider(this.serviceProvider);
            }
            throw new InvalidOperationException($"尚未注册{httpApiType}的token提供者");
        }
    }
}
