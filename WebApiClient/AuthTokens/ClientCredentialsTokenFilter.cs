using System.Threading.Tasks;

namespace WebApiClient.AuthTokens
{
    /// <summary>
    /// 表示提供client_credentials方式的token过滤器
    /// </summary>
    public class ClientCredentialsTokenFilter : AuthTokenFilter
    {
        /// <summary>
        /// 获取或设置提供Token获取的Url节点
        /// 必填
        /// </summary>
        public string TokenEndpoint { get; set; }

        /// <summary>
        /// 获取或设置client_id
        /// 必填
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 获取或设置client_secret
        /// 必填
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// 获取或设置资源范围
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// 获取或设置额外字段，支持字典或模型
        /// </summary>
        public object Extra { get; set; }


        /// <summary>
        /// 请求获取token
        /// 可以使用TokenClient来请求
        /// </summary>
        /// <returns></returns>
        protected override async Task<TokenResult> RequestTokenResultAsync()
        {
            var tokenClient = new TokenClient(this.TokenEndpoint);
            return await tokenClient.RequestClientCredentialsAsync(this.ClientId, this.ClientSecret, this.Scope, this.Extra);
        }

        /// <summary>
        /// 请求刷新token
        /// 可以使用TokenClient来刷新
        /// </summary>
        /// <param name="refresh_token">获取token时返回的refresh_token</param>
        /// <returns></returns>
        protected override async Task<TokenResult> RequestRefreshTokenAsync(string refresh_token)
        {
            var tokenClient = new TokenClient(this.TokenEndpoint);
            return await tokenClient.RequestRefreshTokenAsync(this.ClientId, this.ClientSecret, refresh_token);
        }
    }
}
