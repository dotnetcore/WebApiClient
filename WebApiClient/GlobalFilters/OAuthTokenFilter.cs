using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApiClient.Contexts;
using WebApiClient.Token;

namespace WebApiClient.GlobalFilters
{
    /// <summary>
    /// 表示OAuth授权的token过滤器抽象类
    /// </summary>
    public abstract class OAuthTokenFilter : IApiActionFilter
    {
        /// <summary>
        /// 最近请求token的时间
        /// </summary>
        private DateTime lastTime;

        /// <summary>
        /// 请求到的token
        /// </summary>
        private TokenResult tokenResult;

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
            using (await this.asyncRoot.LockAsync())
            {
                await this.SetAuthorizationAsync(context);
            }
        }

        /// <summary>
        /// 设置授权信息到请求上下文
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>  
        private async Task SetAuthorizationAsync(ApiActionContext context)
        {
            if (this.tokenResult == null)
            {
                this.lastTime = DateTime.Now;
                this.tokenResult = await this.RequestTokenResultAsync();
            }
            else
            {
                await this.RefreshTokenIfExpiresAsync();
            }
            this.AccessTokenResult(context, this.tokenResult);
        }


        /// <summary>
        /// 刷新或重新获取token
        /// </summary>
        /// <returns></returns>
        private async Task RefreshTokenIfExpiresAsync()
        {
            var curExpiresIn = DateTime.Now.Subtract(this.lastTime);
            var tokenExpiresIn = TimeSpan.FromSeconds(this.tokenResult.ExpiresIn);

            if (this.IsExpires(curExpiresIn, tokenExpiresIn) == false)
            {
                return;
            }

            if (string.IsNullOrEmpty(this.tokenResult.RefreshToken) == true)
            {
                this.tokenResult = await this.RequestTokenResultAsync();
            }
            else
            {
                this.tokenResult = await this.RequestRefreshTokenAsync(this.tokenResult.RefreshToken);
            }
        }

        /// <summary>
        /// 计算token是否过期
        /// 默认为在tokenExpiresIn前10分钟就当作过期处理
        /// </summary>
        /// <param name="curExpiresIn">当前的累计时间</param>
        /// <param name="tokenExpiresIn">token的过期日间</param>
        /// <returns></returns>
        protected virtual bool IsExpires(TimeSpan curExpiresIn, TimeSpan tokenExpiresIn)
        {
            return tokenExpiresIn.Subtract(curExpiresIn) < TimeSpan.FromMinutes(10d);
        }


        /// <summary>
        /// 应用AccessToken
        /// 默认为添加到请求头的Authorization
        /// </summary>
        /// <param name="context">请求上下文</param>
        /// <param name="tokenResult">token结果</param>
        protected virtual void AccessTokenResult(ApiActionContext context, TokenResult tokenResult)
        {
            context.RequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.AccessToken);
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
    }
}
