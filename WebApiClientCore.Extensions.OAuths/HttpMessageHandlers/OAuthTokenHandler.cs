using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using WebApiClientCore.HttpMessageHandlers;

namespace WebApiClientCore.Extensions.OAuths.HttpMessageHandlers
{
    /// <summary>
    /// 表示token应用的http消息处理程序
    /// </summary>
    public class OAuthTokenHandler : AuthorizationHandler
    {
        /// <summary>
        /// token提供者
        /// </summary>
        private readonly ITokenProvider tokenProvider;

        /// <summary>
        /// token应用的http消息处理程序
        /// </summary>
        /// <param name="tokenProvider">token提供者</param> 
        public OAuthTokenHandler(ITokenProvider tokenProvider)
        {
            this.tokenProvider = tokenProvider;
        }

        /// <summary>
        /// 设置请求的授权信息
        /// </summary>
        /// <param name="reason">授权原因</param> 
        /// <param name="request">请求</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        protected sealed override async Task SetAuthorizationAsync(SetReason reason, HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (reason == SetReason.ForResend)
            {
                this.tokenProvider.ClearToken();
            }

            var tokenResult = await this.tokenProvider.GetTokenAsync().ConfigureAwait(false);
            this.UseTokenResult(request, tokenResult);
        }

        /// <summary>
        /// 应用token
        /// 默认为添加到请求头的Authorization
        /// </summary>
        /// <param name="request">请求上下文</param>
        /// <param name="tokenResult">token结果</param>
        /// <returns></returns>
        protected virtual void UseTokenResult(HttpRequestMessage request, TokenResult tokenResult)
        {
            var tokenType = tokenResult.Token_type ?? "Bearer";
            request.Headers.Authorization = new AuthenticationHeaderValue(tokenType, tokenResult.Access_token);
        }
    }
}
