using System;
using System.Threading.Tasks;
using WebApiClient.AuthTokens;

namespace Demo
{
    /// <summary>
    /// 表示提供client_credentials方式的token过滤器
    /// </summary>
    public class TokenFilter : AuthTokenFilter
    {
        /// <summary>
        /// 获取提供Token获取的Url节点
        /// </summary>
        public string TokenEndpoint { get; private set; }

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
        /// <param name="tokenEndPoint">提供Token获取的Url节点</param>
        /// <param name="client_id">客户端id</param>
        /// <param name="client_secret">客户端密码</param>
        public TokenFilter(string tokenEndPoint, string client_id, string client_secret)
        {
            this.TokenEndpoint = tokenEndPoint ?? throw new ArgumentNullException(nameof(tokenEndPoint));
            this.ClientId = client_id ?? throw new ArgumentNullException(nameof(client_id));
            this.ClientSecret = client_secret ?? throw new ArgumentNullException(nameof(client_secret));
        }

        /// <summary>
        /// 请求获取token
        /// 可以使用TokenClient来请求
        /// </summary>
        /// <returns></returns>
        protected override async Task<TokenResult> RequestTokenResultAsync()
        {
            using (var tokenClient = new TokenClient(this.TokenEndpoint))
            {
                return await tokenClient.RequestClientCredentialsAsync(this.ClientId, this.ClientSecret);
            }
        }

        /// <summary>
        /// 请求刷新token
        /// 可以使用TokenClient来刷新
        /// </summary>
        /// <param name="refresh_token">获取token时返回的refresh_token</param>
        /// <returns></returns>
        protected override async Task<TokenResult> RequestRefreshTokenAsync(string refresh_token)
        {
            using (var tokenClient = new TokenClient(this.TokenEndpoint))
            {
                return await tokenClient.RequestRefreshTokenAsync(this.ClientId, this.ClientSecret, refresh_token);
            }
        }
    }
}
