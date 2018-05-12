using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.AuthTokens
{
    /// <summary>
    /// 表示OAuth授权的token过滤器抽象类
    /// </summary>
    public abstract class AuthTokenFilter : IApiActionFilter, IDisposable
    {
        /// <summary>
        /// 最近请求到的token
        /// </summary>
        private TokenResult tokenResult;

        /// <summary>
        /// token相关异常
        /// </summary>
        private Exception tokenException;

        /// <summary>
        /// 计时器
        /// </summary>
        private readonly Timer tokenTimer;

        /// <summary>
        /// 异步锁
        /// </summary>
        private readonly AsyncRoot asyncRoot = new AsyncRoot();

        /// <summary>
        /// OAuth授权的token过滤器抽象类
        /// </summary>
        public AuthTokenFilter()
        {
            this.tokenTimer = new Timer(async (state) =>
            {
                using (await this.asyncRoot.LockAsync())
                {
                    await this.RefreshTokenAsync();
                }

                if (this.tokenException != null)
                {
                    var dueTime = this.GetDueTimeSpan(this.tokenResult.ExpiresIn);
                    this.tokenTimer.Change(dueTime, Timeout.InfiniteTimeSpan);
                }
            }, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <returns></returns>
        private async Task RefreshTokenAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(this.tokenResult.RefreshToken) == true)
                {
                    this.tokenResult = await this.RequestTokenResultAsync();
                }
                else
                {
                    this.tokenResult = await this.RequestRefreshTokenAsync(this.tokenResult.RefreshToken);
                }
            }
            catch (Exception ex)
            {
                this.tokenException = ex;
            }
        }

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
                await this.InitTokenIfNullTokenAsync();
            }

            if (this.tokenException != null)
            {
                throw this.tokenException;
            }
            this.AccessTokenResult(context, this.tokenResult);
        }

        /// <summary>
        /// 初始化Token
        /// </summary>
        /// <returns></returns>
        private async Task InitTokenIfNullTokenAsync()
        {
            try
            {
                if (this.tokenResult == null)
                {
                    this.tokenResult = await this.RequestTokenResultAsync();
                    var dueTime = this.GetDueTimeSpan(this.tokenResult.ExpiresIn);
                    this.tokenTimer.Change(dueTime, Timeout.InfiniteTimeSpan);
                }
            }
            catch (Exception ex)
            {
                this.tokenException = ex;
            }
        }

        /// <summary>
        /// 返回Timer延时时间
        /// </summary>
        /// <param name="expiresIn"></param>
        /// <returns></returns>
        private TimeSpan GetDueTimeSpan(long expiresIn)
        {
            return TimeSpan.FromSeconds((double)expiresIn * 0.9d);
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

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.tokenTimer.Dispose();
        }
    }
}
