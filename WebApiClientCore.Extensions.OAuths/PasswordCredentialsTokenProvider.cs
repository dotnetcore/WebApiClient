using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 表示用户名密码身份信息token提供者
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    public class PasswordCredentialsTokenProvider<THttpApi> : TokenProvider
    {
        /// <summary>
        /// 身份信息选项
        /// </summary>
        protected IOptions<PasswordCredentialsOptions<THttpApi>> CredentialsOptions { get; }

        /// <summary>
        /// 用户名密码身份信息token提供者
        /// </summary>
        /// <param name="services"></param>
        /// <param name="credentialsOptions"></param>
        public PasswordCredentialsTokenProvider(IServiceProvider services, IOptions<PasswordCredentialsOptions<THttpApi>> credentialsOptions)
            : base(services)
        {
            this.CredentialsOptions = credentialsOptions;
        }

        /// <summary>
        /// 请求获取token
        /// </summary> 
        /// <param name="oAuthClient">Token客户端</param>
        /// <returns></returns>
        protected override Task<TokenResult> RequestTokenAsync(IOAuthClient oAuthClient)
        {
            var options = this.CredentialsOptions.Value;
            return oAuthClient.RequestTokenAsync(options.Endpoint, options.Credentials);
        }

        /// <summary>
        /// 刷新token
        /// </summary> 
        /// <param name="oAuthClient">Token客户端</param>
        /// <param name="refresh_token">刷新token</param>
        /// <returns></returns>
        protected override Task<TokenResult> RefreshTokenAsync(IOAuthClient oAuthClient, string refresh_token)
        {
            var options = this.CredentialsOptions.Value;
            var credentials = new RefreshTokenCredentials
            {
                Client_id = options.Credentials.Client_id,
                Client_secret = options.Credentials.Client_secret,
                Extra = options.Credentials.Extra,
                Refresh_token = refresh_token
            };

            return oAuthClient.RefreshTokenAsync(options.Endpoint, credentials);
        }
    }
}
