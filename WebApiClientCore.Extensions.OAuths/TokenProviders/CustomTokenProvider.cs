using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using WebApiClientCore.Extensions.OAuths.TokenClients;

namespace WebApiClientCore.Extensions.OAuths.TokenProviders
{
    /// <summary>
    /// 表示指定接口的自定义Token提供者
    /// </summary>
    /// <typeparam name="THttpApi">接口类型</typeparam>
    class CustomTokenProvider<THttpApi> : TokenProvider<THttpApi>
    {
        /// <summary>
        /// 获取提供者类型
        /// </summary>
        public override ProviderType ProviderType => ProviderType.Custom;

        /// <summary>
        /// 指定接口的自定义Token提供者
        /// </summary>
        /// <param name="services"></param>
        public CustomTokenProvider(IServiceProvider services)
            : base(services)
        {
        }

        /// <summary>
        /// 请求获取token
        /// </summary> 
        /// <param name="serviceProvider">服务提供者</param>
        /// <returns></returns>
        protected override Task<TokenResult?> RequestTokenAsync(IServiceProvider serviceProvider)
        {
            return serviceProvider
                .GetRequiredService<ICustomTokenClient<THttpApi>>()
                .RequestTokenAsync();
        }

        /// <summary>
        /// 刷新token
        /// </summary> 
        /// <param name="serviceProvider">服务提供者</param>
        /// <param name="refresh_token">刷新令牌</param>
        /// <returns></returns>
        protected override Task<TokenResult?> RefreshTokenAsync(IServiceProvider serviceProvider, string refresh_token)
        {
            return serviceProvider
               .GetRequiredService<ICustomTokenClient<THttpApi>>()
               .RefreshTokenAsync(refresh_token);
        }
    }
}
