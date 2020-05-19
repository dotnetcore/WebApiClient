using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 表示Token提供者抽象类
    /// </summary>
    public abstract class TokenProvider : IDisposable
    {
        /// <summary>
        /// 最近请求到的token
        /// </summary>
        private TokenResult token;

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
                    this.token = await this.RequestTokenAsync().ConfigureAwait(false);
                }
                else if (this.token.IsExpired() == true)
                {
                    this.token = this.token.CanRefresh() == true
                        ? await this.RefreshTokenAsync(this.token.Refresh_token).ConfigureAwait(false)
                        : await this.RequestTokenAsync().ConfigureAwait(false);
                }

                if (this.token == null)
                {
                    throw new TokenNullException();
                }
                return this.token.EnsureSuccess();
            }
        }

        /// <summary>
        /// 创建OAuth客户端
        /// </summary> 
        /// <returns></returns>
        protected IOAuthClient CreateOAuthClient()
        {
            var options = new HttpApiOptions();
            options.KeyValueSerializeOptions.IgnoreNullValues = true;
            var client = this.services.GetRequiredService<IHttpClientFactory>().CreateClient();
            return HttpApi.Create<IOAuthClient>(client, services, options);
        }

        /// <summary>
        /// 请求获取token
        /// </summary> 
        /// <returns></returns>
        protected abstract Task<TokenResult> RequestTokenAsync();

        /// <summary>
        /// 刷新token
        /// </summary> 
        /// <param name="refresh_token">刷新token</param>
        /// <returns></returns>
        protected abstract Task<TokenResult> RefreshTokenAsync(string refresh_token);

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.asyncRoot.Dispose();
        }
    }
}
