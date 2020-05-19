using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace WebApiClientCore.OAuths
{
    /// <summary>
    /// 表示Client身份信息token提供者
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    public class ClientCredentialsTokenProvider<THttpApi> : TokenProvider
    {
        /// <summary>
        /// 选项
        /// </summary>
        private readonly IOptions<ClientCredentialsOptions<THttpApi>> options;

        /// <summary>
        /// Client身份信息token提供者
        /// </summary>
        /// <param name="options"></param>
        public ClientCredentialsTokenProvider(IOptions<ClientCredentialsOptions<THttpApi>> options)
        {
            this.options = options;
        }

        /// <summary>
        /// 请求获取token
        /// </summary>
        /// <param name="oauthClient"></param>
        /// <returns></returns>
        protected override Task<TokenResult> RequestTokenAsync(IOAuthClient oauthClient)
        {
            var options = this.options.Value;
            return oauthClient.RequestTokenAsync(options.Endpoint, options.Credentials);
        }

        /// <summary>
        /// 请求刷新token
        /// </summary>
        /// <param name="oauthClient"></param> 
        /// <param name="refresh_token">刷新token</param>
        /// <returns></returns>
        protected override Task<TokenResult> RefreshTokenAsync(IOAuthClient oauthClient, string refresh_token)
        {
            var options = this.options.Value;
            var credentials = new RefreshTokenCredentials
            {
                Client_id = options.Credentials.Client_id,
                Client_secret = options.Credentials.Client_secret,
                Extra = options.Credentials.Extra,
                Refresh_token = refresh_token
            };

            return oauthClient.RefreshTokenAsync(options.Endpoint, credentials);
        }
    }
}
