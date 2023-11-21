using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiClientCore.Extensions.OAuths.Exceptions;

namespace WebApiClientCore.Extensions.OAuths.TokenProviders
{
    /// <summary>
    /// 表示多应用下Token提供者抽象类
    /// </summary>
    public abstract class DynamicTokenProvider : IDynamicTokenProvider
    {
        /// <summary>
        /// token缓存
        /// </summary>
        private static readonly ConcurrentDictionary<string, TokenResult> tokenResults
            = new ConcurrentDictionary<string, TokenResult>();

        /// <summary>
        /// 服务提供者
        /// </summary>
        private readonly IServiceProvider _services;

        /// <summary>
        /// 异步锁
        /// </summary>
        private readonly AsyncRoot asyncRoot = new AsyncRoot();

        /// <summary>
        /// 获取或设置别名
        /// </summary>
        public string Name { get; set; } = Options.DefaultName;

        /// <summary>
        /// 表示多应用Token提供者抽象类
        /// </summary>
        protected DynamicTokenProvider(IServiceProvider service)
        {
            _services = service;
        }

        private const string _default_identifier = "default";

        /// <summary>
        /// 强制清除token以支持下次获取到新的token
        /// </summary>
        public void ClearToken()
        {
            ClearTokenAsync(_default_identifier).Wait();
        }

        /// <summary>
        /// 强制清除token以支持下次获取到新的token
        /// </summary>
        /// <param name="identifier">应用标识</param>
        public async Task ClearTokenAsync(string identifier)
        {
            using (await asyncRoot.LockAsync().ConfigureAwait(false))
            {
                identifier ??= _default_identifier;
                if (tokenResults.ContainsKey(identifier))
                    tokenResults.Remove(identifier, out _);
            }
        }

        /// <summary>
        /// 根据应用标识获取token信息
        /// </summary>
        public async Task<TokenResult> GetTokenAsync(string identifier)
        {
            using (await asyncRoot.LockAsync().ConfigureAwait(false))
            {
                identifier ??= _default_identifier;
                if (!tokenResults.TryGetValue(identifier, out var token) || token.IsExpired())
                {
                    using var scope = _services.CreateScope();

                    token = (token?.CanRefresh() != true)
                        ? await RequestTokenAsync(scope.ServiceProvider).ConfigureAwait(false)
                        : await RefreshTokenAsync(scope.ServiceProvider, token.Refresh_token!).ConfigureAwait(false);

                    if (token == null)
                    {
                        throw new TokenNullException();
                    }

                    tokenResults.GetOrAdd(identifier, token);
                    return token.EnsureSuccess();
                }
                return token;
            }
        }

        /// <summary>
        /// 获取token信息
        /// </summary>
        public async Task<TokenResult> GetTokenAsync()
        {
            return await GetTokenAsync(_default_identifier).ConfigureAwait(false);
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
        /// 转换为string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name}";
        }
    }
}