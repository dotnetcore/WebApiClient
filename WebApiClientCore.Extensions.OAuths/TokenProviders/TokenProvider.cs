using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
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
        public TOptions GetOptionsValue<TOptions>()
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
        /// 获取 token 信息
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
                else if (this.ShouldRefreshToken(this.token))
                {
                    using var scope = this.services.CreateScope();

                    this.token = this.token.CanRefresh() == false
                        ? await this.RequestTokenAsync(scope.ServiceProvider).ConfigureAwait(false)
                        : await this.RefreshTokenAsync(scope.ServiceProvider, this.token.Refresh_token ?? string.Empty).ConfigureAwait(false);
                }

                return this.token == null ? throw new TokenNullException() : this.token.EnsureSuccess();
            }
        }

        /// <summary>
        /// 判断是否应该刷新Token
        /// 混合方案：优先从独立配置读取，其次从 HttpApiOptions.Properties 读取，最后使用默认行为
        /// </summary>
        /// <param name="token">token结果</param>
        /// <returns></returns>
        private bool ShouldRefreshToken(TokenResult token)
        {
            // 获取配置
            var tokenOptions = this.GetTokenRefreshOptions();
            
            if (!tokenOptions.UseTokenRefreshWindow)
            {
                // 如果禁用刷新窗口,使用原有行为
                return token.IsExpired();
            }

            // 计算刷新窗口
            var refreshWindow = this.CalculateRefreshWindow(token, tokenOptions);
            return token.IsExpired(refreshWindow);
        }

        /// <summary>
        /// 获取Token刷新配置
        /// 优先级：独立配置(IOptionsMonitor&lt;OAuthTokenOptions&gt;) > HttpApiOptions.Properties > 默认配置
        /// </summary>
        /// <returns>Token刷新配置</returns>
        private OAuthTokenOptions GetTokenRefreshOptions()
        {
            // 优先从独立配置读取 (方案2：独立配置类)
            var oauthOptionsMonitor = this.services.GetService<IOptionsMonitor<OAuthTokenOptions>>();
            if (oauthOptionsMonitor != null)
            {
                try
                {
                    var options = oauthOptionsMonitor.Get(this.Name);
                    // 检查是否为默认配置，如果不是则使用
                    if (options != null)
                    {
                        return options;
                    }
                }
                catch
                {
                    // 如果获取失败，继续尝试下一种方式
                }
            }

            // 其次从 HttpApiOptions.Properties 读取 (方案1：Properties 字典)
            var httpApiOptionsMonitor = this.services.GetService<IOptionsMonitor<HttpApiOptions>>();
            if (httpApiOptionsMonitor != null)
            {
                try
                {
                    var httpApiOptions = httpApiOptionsMonitor.Get(this.Name);
                    if (httpApiOptions != null)
                    {
                        return httpApiOptions.GetOAuthTokenOptions();
                    }
                }
                catch
                {
                    // 如果获取失败，使用默认配置
                }
            }

            // 最后使用默认配置
            return new OAuthTokenOptions();
        }

        /// <summary>
        /// 根据配置策略计算刷新窗口
        /// </summary>
        /// <param name="token">Token结果</param>
        /// <param name="options">配置选项</param>
        /// <returns>刷新窗口时间</returns>
        private TimeSpan CalculateRefreshWindow(TokenResult token, OAuthTokenOptions options)
        {
            var fixedWindow = TimeSpan.FromSeconds(options.RefreshWindowSeconds);
            var percentageWindow = TimeSpan.FromSeconds(token.Expires_in * options.RefreshWindowPercentage);

            return options.RefreshWindowStrategy switch
            {
                RefreshWindowStrategy.FixedSeconds => fixedWindow,
                RefreshWindowStrategy.Percentage => percentageWindow,
                RefreshWindowStrategy.Auto => fixedWindow < percentageWindow ? fixedWindow : percentageWindow,
                _ => fixedWindow < percentageWindow ? fixedWindow : percentageWindow // 默认使用Auto
            };
        }


        /// <summary>
        /// 请求获取 token
        /// </summary> 
        /// <param name="serviceProvider">服务提供者</param>
        /// <returns></returns>
        protected abstract Task<TokenResult?> RequestTokenAsync(IServiceProvider serviceProvider);

        /// <summary>
        /// 刷新 token
        /// </summary> 
        /// <param name="serviceProvider">服务提供者</param>
        /// <param name="refresh_token">刷新 token</param>
        /// <returns></returns>
        protected abstract Task<TokenResult?> RefreshTokenAsync(IServiceProvider serviceProvider, string refresh_token);

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
