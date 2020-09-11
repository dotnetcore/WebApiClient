using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using WebApiClientCore.Extensions.OAuths.TokenClients;

namespace WebApiClientCore.Extensions.OAuths.TokenProviders
{
    /// <summary>
    /// 表示用户名密码身份信息token提供者
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    public class PasswordCredentialsTokenProvider<THttpApi> : TokenProvider, IPasswordCredentialsTokenProvider<THttpApi>
    {
        /// <summary>
        /// 用户名密码身份信息token提供者
        /// </summary>
        /// <param name="services"></param>
        public PasswordCredentialsTokenProvider(IServiceProvider services)
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
            var custom = serviceProvider.GetService<ICustomTokenClient<THttpApi>>();
            if (custom != null)
            {
                return custom.RequestTokenAsync();
            }

            return serviceProvider
               .GetRequiredService<IPasswordCredentialsTokenClient>()
               .RequestTokenAsync(typeof(THttpApi));
        }

        /// <summary>
        /// 刷新token
        /// </summary> 
        /// <param name="serviceProvider">服务提供者</param>
        /// <param name="refresh_token">刷新令牌</param>
        /// <returns></returns>
        protected override Task<TokenResult?> RefreshTokenAsync(IServiceProvider serviceProvider, string refresh_token)
        {
            var custom = serviceProvider.GetService<ICustomTokenClient<THttpApi>>();
            if (custom != null)
            {
                return custom.RequestTokenAsync();
            }

            return serviceProvider
               .GetRequiredService<IPasswordCredentialsTokenClient>()
               .RefreshTokenAsync(refresh_token, typeof(THttpApi));
        }
    }
}
