using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using WebApiClientCore.Extensions.OAuths.Exceptions;

namespace WebApiClientCore.Extensions.OAuths.TokenProviders
{
    /// <summary>
    /// 表示 token 提供者抽象类
    /// </summary>
    public abstract class TokenProvider : ITokenProvider
    {
        /// <summary>
        /// 最近请求到的 token
        /// </summary>
        private TokenResult? token;

        /// <summary>
        /// 服务提供者
        /// </summary>
        private readonly IServiceProvider services;

        /// <summary>
        /// 异步锁
        /// </summary>
        private readonly AsyncRoot asyncRoot = new();

        /// <summary>
        /// 获取或设置名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// token 提供者抽象类
        /// </summary>
        /// <param name="services"></param>
        public TokenProvider(IServiceProvider services)
        {
            this.services = services;
        }

        /// <summary>
        /// 获取选项值
        /// Options 名称为本类型的 Name 属性
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <returns></returns>
        public TOptions GetOptionsValue<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TOptions>()
        {
            return this.services.GetRequiredService<IOptionsMonitor<TOptions>>().Get(this.Name);
        }

        /// <summary>
        /// 强制清除 token 以支持下次获取到新的 token
        /// </summary>
        public void ClearToken()
        {
            using (this.asyncRoot.Lock())
            {
                this.token = null;
            }
        }

        /// <summary>
        /// 强制清除 token 以支持下次获取到新的 token
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ClearTokenAsync(CancellationToken cancellationToken = default)
        {
            using (await this.asyncRoot.LockAsync(cancellationToken))
            {
                this.token = null;
            }
        }

        /// <summary>
        /// 获取 token 信息
        /// </summary> 
        /// <returns></returns>
        public Task<TokenResult> GetTokenAsync()
        {
            return this.GetTokenAsync(CancellationToken.None);
        }

        /// <summary>
        /// 获取 token 信息
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<TokenResult> GetTokenAsync(CancellationToken cancellationToken)
        {
            using (await this.asyncRoot.LockAsync(cancellationToken).ConfigureAwait(false))
            {
                if (this.token == null)
                {
                    using var scope = this.services.CreateScope();
                    this.token = await this.RequestTokenAsync(scope.ServiceProvider, cancellationToken).ConfigureAwait(false);
                }
                else if (this.token.IsExpired() == true)
                {
                    using var scope = this.services.CreateScope();

                    this.token = this.token.CanRefresh() == false
                        ? await this.RequestTokenAsync(scope.ServiceProvider, cancellationToken).ConfigureAwait(false)
                        : await this.RefreshTokenAsync(scope.ServiceProvider, this.token.Refresh_token ?? string.Empty, cancellationToken).ConfigureAwait(false);
                }

                return this.token == null ? throw new TokenNullException() : this.token.EnsureSuccess();
            }
        }


        /// <summary>
        /// 请求获取 token
        /// </summary> 
        /// <param name="serviceProvider">服务提供者</param>
        /// <returns></returns>
        [Obsolete("请使用 RequestTokenAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken) 方法")]
        protected virtual Task<TokenResult?> RequestTokenAsync(IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 刷新 token
        /// </summary> 
        /// <param name="serviceProvider">服务提供者</param>
        /// <param name="refresh_token">刷新 token</param>
        /// <returns></returns>
        [Obsolete("请使用 RefreshTokenAsync(IServiceProvider serviceProvider, string refresh_token, CancellationToken cancellationToken) 方法")]
        protected virtual Task<TokenResult?> RefreshTokenAsync(IServiceProvider serviceProvider, string refresh_token)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 请求获取 token
        /// </summary> 
        /// <param name="serviceProvider">服务提供者</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected virtual Task<TokenResult?> RequestTokenAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            return this.RequestTokenAsync(serviceProvider);
        }

        /// <summary>
        /// 刷新 token
        /// </summary> 
        /// <param name="serviceProvider">服务提供者</param>
        /// <param name="refresh_token">刷新 token</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected virtual Task<TokenResult?> RefreshTokenAsync(IServiceProvider serviceProvider, string refresh_token, CancellationToken cancellationToken)
        {
            return this.RefreshTokenAsync(serviceProvider, refresh_token);
        }

        /// <summary>
        /// 转换为 string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
