using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;
using WebApiClientCore.OAuths;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示OAuth授权的token过滤器抽象
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public abstract class TokenFilterAttribute : ApiFilterAttribute
    {
        /// <summary>
        /// 最近请求到的token
        /// </summary>
        private TokenResult token;

        /// <summary>
        /// 异步锁
        /// </summary>
        private readonly AsyncRoot asyncRoot = new AsyncRoot();

        /// <summary>
        /// 请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public sealed override async Task OnRequestAsync(ApiRequestContext context)
        {
            using (await this.asyncRoot.LockAsync().ConfigureAwait(false))
            {
                await this.InitOrRefreshTokenAsync(context).ConfigureAwait(false);
            }
            this.UseTokenResult(context, this.token);
        }

        /// <summary>
        /// 初始化或刷新token
        /// </summary>
        /// <exception cref="HttpApiTokenException"></exception>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task InitOrRefreshTokenAsync(ApiRequestContext context)
        {
            if (this.token == null)
            {
                this.token = await this.RequestTokenAsync(context).ConfigureAwait(false);
            }
            else if (this.token.IsExpired() == true)
            {
                this.token = this.token.CanRefresh() == true
                    ? await this.RefreshTokenAsync(context, this.token.Refresh_token).ConfigureAwait(false)
                    : await this.RequestTokenAsync(context).ConfigureAwait(false);
            }

            if (this.token == null)
            {
                throw new HttpApiTokenException(Resx.cannot_GetToken);
            }
            this.token.EnsureSuccess();
        }

        /// <summary>
        /// 应用token
        /// 默认为添加到请求头的Authorization
        /// </summary>
        /// <param name="context">请求上下文</param>
        /// <param name="tokenResult">token结果</param>
        /// <returns></returns>
        protected virtual void UseTokenResult(ApiRequestContext context, TokenResult tokenResult)
        {
            var tokenType = tokenResult.Token_type ?? "Bearer";
            context.HttpContext.RequestMessage.Headers.Authorization = new AuthenticationHeaderValue(tokenType, tokenResult.Access_token);
        }

        /// <summary>
        /// 请求获取token
        /// 可以使用TokenClient来请求
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected abstract Task<TokenResult> RequestTokenAsync(ApiRequestContext context);

        /// <summary>
        /// 请求刷新token
        /// 可以使用TokenClient来刷新
        /// </summary>
        /// <param name="context"></param>
        /// <param name="refresh_token">获取token时返回的refresh_token</param>
        /// <returns></returns>
        protected abstract Task<TokenResult> RefreshTokenAsync(ApiRequestContext context, string refresh_token);
    }
}
