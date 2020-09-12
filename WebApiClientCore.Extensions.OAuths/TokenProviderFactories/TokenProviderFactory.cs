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
        /// 创建token提供者
        /// </summary>
        /// <param name="httpApiType">接口名称</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public ITokenProvider Create(Type httpApiType)
        {
            var name = HttpApi.GetName(httpApiType);
            return this.Create(name);
        }

        /// <summary>
        /// 创建token提供者
        /// </summary>
        /// <param name="name">提供者的别名</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public ITokenProvider Create(string name)
        {
            if (this.options.TryGetValue(name, out var tokenProviderType))
            {
                return (ITokenProvider)this.serviceProvider.GetRequiredService(tokenProviderType);
            }
            throw new InvalidOperationException($"未注册别名为{name}的token提供者");
        }
    }
}
