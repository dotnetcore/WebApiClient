using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using WebApiClientCore.Extensions.OAuths.Exceptions;
using WebApiClientCore.Extensions.OAuths.TokenClients;

namespace WebApiClientCore.Extensions.OAuths.TokenProviders
{
    /// <summary>
    /// 表示Client身份信息token提供者
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    public class ClientCredentialsTokenProvider<THttpApi> : TokenProvider<THttpApi>
    {
        /// <summary>
        /// 获取提供者类型
        /// </summary>
        public override ProviderType ProviderType => ProviderType.ClientCredentials;

        /// <summary>
        /// Client身份信息token提供者
        /// </summary>
        /// <param name="services"></param> 
        public ClientCredentialsTokenProvider(IServiceProvider services)
            : base(services)
        {
        }

        /// <summary>
        /// 请求获取token
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        protected override Task<TokenResult?> RequestTokenAsync(IServiceProvider serviceProvider)
        {
            var name = HttpApi.GetName<THttpApi>();
            var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<ClientCredentialsOptions>>();

            var options = optionsMonitor.Get(name);
            if (options.Endpoint == null)
            {
                throw new TokenEndPointNullException();
            }

            var clientApi = serviceProvider.GetRequiredService<IOAuthTokenClientApi>();
            return clientApi.RequestTokenAsync(options.Endpoint, options.Credentials);
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="refresh_token"></param>
        /// <returns></returns>
        protected override Task<TokenResult?> RefreshTokenAsync(IServiceProvider serviceProvider, string refresh_token)
        {
            var name = HttpApi.GetName<THttpApi>();
            var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<ClientCredentialsOptions>>();

            var options = optionsMonitor.Get(name);
            if (options.Endpoint == null)
            {
                throw new TokenEndPointNullException();
            }

            if (options.UseRefreshToken == false)
            {
                return this.RequestTokenAsync(serviceProvider);
            }

            var refreshCredentials = new RefreshTokenCredentials
            {
                Client_id = options.Credentials.Client_id,
                Client_secret = options.Credentials.Client_secret,
                Extra = options.Credentials.Extra,
                Refresh_token = refresh_token
            };

            var clientApi = serviceProvider.GetRequiredService<IOAuthTokenClientApi>();
            return clientApi.RefreshTokenAsync(options.Endpoint, refreshCredentials);
        }
    }
}