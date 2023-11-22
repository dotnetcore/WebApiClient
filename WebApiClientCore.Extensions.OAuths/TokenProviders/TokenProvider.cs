using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using WebApiClientCore.Extensions.OAuths.Exceptions;

namespace WebApiClientCore.Extensions.OAuths.TokenProviders
{
    /// <summary>
    /// 表示Token提供者抽象类
    /// </summary>
    public abstract class TokenProvider : ITokenProvider
    {
        /// <summary>
        /// token缓存
        /// </summary>
        private static readonly ConcurrentDictionary<string, Lazy<TokenResult?>> keyTokens
            = new ConcurrentDictionary<string, Lazy<TokenResult?>>();

        /// <summary>
        /// 服务提供者
        /// </summary>
        private readonly IServiceProvider services;

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
            ClearToken(string.Empty);
        }
        /// <summary>
        /// 强制清除token以支持下次获取到新的token
        /// </summary>
        /// <param name="identifier">应用标识</param>
        public void ClearToken(string identifier)
        {
            keyTokens.TryRemove(identifier, out _);
        }

        /// <summary>
        /// 获取token信息
        /// </summary>
        /// <returns></returns>
        public async Task<TokenResult> GetTokenAsync()
        {
            return await GetTokenAsync(string.Empty);
        }

        /// <summary>
        /// 获取token信息
        /// </summary>
        /// <param name="identifier">应用标识</param>
        public async Task<TokenResult> GetTokenAsync(string identifier)
        {
            if (!keyTokens.TryGetValue(identifier, out var tokenItem)
                || tokenItem?.Value?.IsExpired() == true)
            {
                using var scope = services.CreateScope();

                var token = (
                    tokenItem?.Value?.CanRefresh() != true
                        ? await RequestTokenAsync(scope.ServiceProvider, identifier)
                            .ConfigureAwait(false)
                        : await RefreshTokenAsync(scope.ServiceProvider, tokenItem.Value.Refresh_token!)
                            .ConfigureAwait(false)
                    ) ?? throw new TokenNullException();

                tokenItem = new Lazy<TokenResult?>(() => token);

                keyTokens.AddOrUpdate(identifier, tokenItem, (key, old) => tokenItem);

                return token.EnsureSuccess();
            }

            return tokenItem?.Value!;
        }


        /// <summary>
        /// 请求获取token
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        /// <returns></returns>
        protected abstract Task<TokenResult?> RequestTokenAsync(IServiceProvider serviceProvider);

        /// <summary>
        /// 请求获取token
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        /// <param name="identifier">服务提供者</param>
        protected virtual Task<TokenResult?> RequestTokenAsync(IServiceProvider serviceProvider, string? identifier)
        {
            if (string.IsNullOrEmpty(identifier))
                return RequestTokenAsync(serviceProvider);

            throw new NotImplementedException();
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        /// <param name="refresh_token">刷新token</param>
        /// <returns></returns>
        protected abstract Task<TokenResult?> RefreshTokenAsync(IServiceProvider serviceProvider, string refresh_token);

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
