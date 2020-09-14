using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using WebApiClientCore.Extensions.OAuths.Exceptions;

namespace WebApiClientCore.Extensions.OAuths.TokenProviders
{
    /// <summary>
    /// 表示Token提供者抽象类
    /// </summary>
    public abstract class TokenProvider : Disposable, ITokenProvider
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
        /// 获取或设置别名
        /// </summary>
        public string Name { get; set; } = Options.DefaultName;

        /// <summary>
        /// Token提供者抽象类
        /// </summary>
        /// <param name="services"></param>
        public TokenProvider(IServiceProvider services)
        {
            this.services = services;
        }

        /// <summary>
        /// 获取选项值
        /// Options名称为本类型的Name属性
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <returns></returns>
        public TOptions GetOptionsValue<TOptions>()
        {
            return this.services.GetRequiredService<IOptionsMonitor<TOptions>>().Get(this.Name);
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
                    this.token = await this.RequestTokenAsync(scope.ServiceProvider).ConfigureAwait(false);
                }
                else if (this.token.IsExpired() == true)
                {
                    using var scope = this.services.CreateScope();

                    this.token = this.token.CanRefresh() == false
                        ? await this.RequestTokenAsync(scope.ServiceProvider).ConfigureAwait(false)
                        : await this.RefreshTokenAsync(scope.ServiceProvider, this.token.Refresh_token ?? string.Empty).ConfigureAwait(false);
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
        /// <param name="serviceProvider">服务提供者</param>
        /// <returns></returns>
        protected abstract Task<TokenResult?> RequestTokenAsync(IServiceProvider serviceProvider);

        /// <summary>
        /// 刷新token
        /// </summary> 
        /// <param name="serviceProvider">服务提供者</param>
        /// <param name="refresh_token">刷新token</param>
        /// <returns></returns>
        protected abstract Task<TokenResult?> RefreshTokenAsync(IServiceProvider serviceProvider, string refresh_token);

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            this.asyncRoot.Dispose();
        }

        /// <summary>
        /// 转换为string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
