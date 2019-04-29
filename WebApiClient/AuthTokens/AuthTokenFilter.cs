using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.AuthTokens
{
    /// <summary>
    /// 表示OAuth授权的token过滤器抽象类
    /// </summary>
    public abstract class AuthTokenFilter : Disposable, IApiActionFilter
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
        /// 请求完成之后
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        Task IApiActionFilter.OnEndRequestAsync(ApiActionContext context)
        {
            return ApiTask.CompletedTask;
        }

        /// <summary>
        /// 准备请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        async Task IApiActionFilter.OnBeginRequestAsync(ApiActionContext context)
        {
            using (await this.asyncRoot.LockAsync().ConfigureAwait(false))
            {
                await this.InitOrRefreshTokenAsync().ConfigureAwait(false);
            }

            this.AccessTokenResult(context, this.token);
        }

        /// <summary>
        /// 初始化或刷新token
        /// </summary>
        /// <exception cref="HttpApiTokenException"></exception>
        /// <returns></returns>
        private async Task InitOrRefreshTokenAsync()
        {
            if (this.token == null)
            {
                this.token = await this.RequestTokenResultAsync().ConfigureAwait(false);
            }
            else if (this.token.IsExpired() == true)
            {
                if (this.token.CanRefresh() == true)
                {
                    this.token = await this.RequestRefreshTokenAsync(this.token.RefreshToken).ConfigureAwait(false);
                }
                else
                {
                    this.token = await this.RequestTokenResultAsync().ConfigureAwait(false);
                }
            }
            this.token.EnsureSuccess();
        }

        /// <summary>
        /// 应用AccessToken
        /// 默认为添加到请求头的Authorization
        /// </summary>
        /// <param name="context">请求上下文</param>
        /// <param name="tokenResult">token结果</param>
        protected virtual void AccessTokenResult(ApiActionContext context, TokenResult tokenResult)
        {
            var tokenType = tokenResult.TokenType ?? "Bearer";
            context.RequestMessage.Headers.Authorization = new AuthenticationHeaderValue(tokenType, tokenResult.AccessToken);
        }

        /// <summary>
        /// 请求获取token
        /// 可以使用TokenClient来请求
        /// </summary>
        /// <returns></returns>
        protected abstract Task<TokenResult> RequestTokenResultAsync();

        /// <summary>
        /// 请求刷新token
        /// 可以使用TokenClient来刷新
        /// </summary>
        /// <param name="refresh_token">获取token时返回的refresh_token</param>
        /// <returns></returns>
        protected abstract Task<TokenResult> RequestRefreshTokenAsync(string refresh_token);

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否也释放托管资源</param>
        protected override void Dispose(bool disposing)
        {
            this.asyncRoot.Dispose();
        }
    }
}
