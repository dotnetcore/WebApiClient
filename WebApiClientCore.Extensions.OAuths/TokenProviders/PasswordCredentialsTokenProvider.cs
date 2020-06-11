using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using WebApiClientCore.Extensions.OAuths.Exceptions;

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
            var options = this.GetCredentialsOptions(serviceProvider);
            if (options.Endpoint == null)
            {
                throw new TokenEndPointNullException();
            }

            var oAuthClient = serviceProvider.GetRequiredService<IOAuthClient>();
            return oAuthClient.RequestTokenAsync(options.Endpoint, options.Credentials);
        }

        /// <summary>
        /// 刷新token
        /// </summary> 
        /// <param name="serviceProvider">服务提供者</param>
        /// <param name="refresh_token">刷新token</param>
        /// <returns></returns>
        protected override Task<TokenResult?> RefreshTokenAsync(IServiceProvider serviceProvider, string? refresh_token)
        {
            var options = this.GetCredentialsOptions(serviceProvider);
            if (options.Endpoint == null)
            {
                throw new TokenEndPointNullException();
            }

            if (options.UseRefreshToken == false)
            {
                return this.RequestTokenAsync(serviceProvider);
            }

            var credentials = new RefreshTokenCredentials
            {
                Client_id = options.Credentials.Client_id,
                Client_secret = options.Credentials.Client_secret,
                Extra = options.Credentials.Extra,
                Refresh_token = refresh_token
            };

            var oAuthClient = serviceProvider.GetRequiredService<IOAuthClient>();
            return oAuthClient.RefreshTokenAsync(options.Endpoint, credentials);
        }

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        private PasswordCredentialsOptions GetCredentialsOptions(IServiceProvider serviceProvider)
        {
            var name = typeof(THttpApi).FullName;
            return serviceProvider.GetService<IOptionsMonitor<PasswordCredentialsOptions>>().Get(name);
        }
    }
}
