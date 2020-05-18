using System.Threading.Tasks;
using WebApiClientCore.OAuths;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示提供password身份方式的token过滤器抽象
    /// </summary>
    public abstract class PasswordTokenFilterAttribue : TokenFilterAttribute
    {
        /// <summary>
        /// 使用OAuthClient请求获取token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override Task<TokenResult> RequestTokenAsync(ApiRequestContext context)
        {
            var options = this.GetPasswordCredentialsOptions(context);
            var oauthClient = context.HttpContext.CreateOAuthClient();
            return oauthClient.RequestTokenAsync(options.Endpoint, options.Credentials);
        }

        /// <summary>
        /// 使用OAuthClient请求刷新token
        /// </summary>
        /// <param name="context"></param>
        /// <param name="refresh_token">刷新token</param>
        /// <returns></returns>
        protected override Task<TokenResult> RefreshTokenAsync(ApiRequestContext context, string refresh_token)
        {
            var options = this.GetPasswordCredentialsOptions(context);
            var credentials = new RefreshTokenCredentials
            {
                Client_id = options.Credentials.Client_id,
                Client_secret = options.Credentials.Client_secret,
                Extra = options.Credentials.Extra,
                Refresh_token = refresh_token
            };

            var oauthClient = context.HttpContext.CreateOAuthClient();
            return oauthClient.RefreshTokenAsync(options.Endpoint, credentials);
        }

        /// <summary>
        /// 获取用户名密码身份信息选项
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected abstract PasswordCredentialsOptions GetPasswordCredentialsOptions(ApiRequestContext context);
    }
}
