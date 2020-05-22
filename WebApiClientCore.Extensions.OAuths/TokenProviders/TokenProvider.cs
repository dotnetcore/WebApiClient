using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using WebApiClientCore.Extensions.OAuths.Exceptions;

namespace WebApiClientCore.Extensions.OAuths.TokenProviders
{
    /// <summary>
    /// 表示Token提供者抽象类
    /// </summary>
    public abstract class TokenProvider : ITokenProvider, IDisposable
    {
        /// <summary>
        /// 最近请求到的token
        /// </summary>
        private TokenResult? token;

        /// <summary>
        /// 服务提供者
        /// </summary>
        private readonly IServiceProvider services;

        /// <summary>
        /// 异步锁
        /// </summary>
        private readonly AsyncRoot asyncRoot = new AsyncRoot();

        /// <summary>
        /// Token提供者抽象类
        /// </summary>
        /// <param name="services"></param>
        public TokenProvider(IServiceProvider services)
        {
            this.services = services;
        }

        /// <summary>
        /// 强制清除token以支持下次获取到新的token
        /// </summary>
        public void ClearToken()
        {
            using (this.asyncRoot.Lock())
            {
                this.token = null;
            }
        }

        /// <summary>
        /// 获取token信息
        /// </summary> 
        /// <returns></returns>
        public async Task<TokenResult> GetTokenAsync()
        {
            using (await this.asyncRoot.LockAsync().ConfigureAwait(false))
            {
                if (this.token == null)
                {
                    using var scope = this.services.CreateScope();
                    var oAuthClient = scope.ServiceProvider.GetRequiredService<IOAuthClient>();
                    this.token = await this.RequestTokenAsync(oAuthClient).ConfigureAwait(false);
                }
                else if (this.token.IsExpired() == true)
                {
                    using var scope = this.services.CreateScope();
                    var oAuthClient = scope.ServiceProvider.GetRequiredService<IOAuthClient>();
                    this.token = this.token.CanRefresh() == false ?
                        await this.RequestTokenAsync(oAuthClient).ConfigureAwait(false) :
                        await this.RefreshTokenAsync(oAuthClient, this.token.Refresh_token).ConfigureAwait(false);
                }

                if (this.token == null)
                {
                    throw new TokenNullException();
                }
                return this.token.EnsureSuccess();
            }
        }


        /// <summary>
        /// 请求获取token
        /// </summary> 
        /// <param name="oAuthClient">Token客户端</param>
        /// <returns></returns>
        protected abstract Task<TokenResult> RequestTokenAsync(IOAuthClient oAuthClient);

        /// <summary>
        /// 刷新token
        /// </summary> 
        /// <param name="oAuthClient">Token客户端</param>
        /// <param name="refresh_token">刷新token</param>
        /// <returns></returns>
        protected abstract Task<TokenResult> RefreshTokenAsync(IOAuthClient oAuthClient, string? refresh_token);

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.asyncRoot.Dispose();
        }
    }
}
