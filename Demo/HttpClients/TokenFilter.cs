using System.Threading.Tasks;
using WebApiClient.AuthTokens;

namespace Demo.HttpClients
{
    /// <summary>
    /// auth2 token全局过滤器
    /// 用法：HttpApiConfig.GlobalFilters.Add( new TokenFilter() )
    /// </summary>
    class TokenFilter : AuthTokenFilter
    {
        private readonly ITokenToken tokenClient = TokenClient.Get("http://localhost:5000/connect/token");

        /// <summary>
        /// 获取client_id
        /// </summary>
        public string ClientId { get; private set; }

        /// <summary>
        /// 获取client_secret
        /// </summary>
        public string ClientSecret { get; private set; }

        /// <summary>
        /// OAuth授权的token过滤器
        /// </summary>
        /// <param name="client_id">客户端id</param>
        /// <param name="client_secret">客户端密码</param>
        public TokenFilter(string client_id, string client_secret)
        {
            this.ClientId = client_id;
            this.ClientSecret = client_secret;
        }

        protected override async Task<TokenResult> RequestTokenResultAsync()
        {
            return await this.tokenClient.RequestClientCredentialsAsync(this.ClientId, this.ClientSecret);
        }

        protected override async Task<TokenResult> RequestRefreshTokenAsync(string refresh_token)
        {
            return await this.tokenClient.RequestRefreshTokenAsync(this.ClientSecret, refresh_token);
        }
    }
}
