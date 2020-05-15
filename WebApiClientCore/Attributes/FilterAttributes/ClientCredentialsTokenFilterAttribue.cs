using System.Threading.Tasks;
using WebApiClientCore.Tokens;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示提供client_credentials方式的token过滤器抽象
    /// </summary>
    public abstract class ClientCredentialsTokenFilterAttribue : TokenFilterAttribute
    {
        /// <summary>
        /// 使用TokenClient请求获取token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override async Task<TokenResult> RequestTokenAsync(ApiRequestContext context)
        {
            var credentials = this.GetClientCredentials(context);
            var tokenClient = new TokenClient(credentials.TokenEndpoint);
            return await tokenClient.RequestClientCredentialsAsync(credentials.ClientId, credentials.ClientSecret, credentials.Scope, credentials.Extra);
        }

        /// <summary>
        /// 使用TokenClient请求刷新token
        /// </summary>
        /// <param name="context"></param>
        /// <param name="refresh_token">刷新token</param>
        /// <returns></returns>
        protected override async Task<TokenResult> RequestRefreshTokenAsync(ApiRequestContext context, string refresh_token)
        {
            var credentials = this.GetClientCredentials(context);
            var tokenClient = new TokenClient(credentials.TokenEndpoint);
            return await tokenClient.RequestRefreshTokenAsync(credentials.ClientId, credentials.ClientSecret, refresh_token, credentials.Extra);
        }

        /// <summary>
        /// 获取Client身份信息
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected abstract ClientCredentials GetClientCredentials(ApiRequestContext context);
    }
}
