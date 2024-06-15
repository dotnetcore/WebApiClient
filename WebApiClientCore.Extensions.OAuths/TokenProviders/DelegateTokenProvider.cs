using System;
using System.Threading.Tasks;

namespace WebApiClientCore.Extensions.OAuths.TokenProviders
{
    /// <summary>
    /// 表示指定委托请求 token 提供者
    /// </summary>
    sealed class DelegateTokenProvider : TokenProvider
    {
        /// <summary>
        /// token请求委托
        /// </summary>
        private readonly Func<IServiceProvider, Task<TokenResult?>> tokenRequest;

        /// <summary>
        /// 指定委托请求 token 提供者
        /// </summary>
        /// <param name="services"></param>
        /// <param name="tokenRequest">token请求委托</param>
        public DelegateTokenProvider(IServiceProvider services, Func<IServiceProvider, Task<TokenResult?>> tokenRequest)
            : base(services)
        {
            this.tokenRequest = tokenRequest;
        }

        /// <summary>
        /// 请求获取 token
        /// </summary> 
        /// <param name="serviceProvider">服务提供者</param>
        /// <returns></returns>
        protected override Task<TokenResult?> RequestTokenAsync(IServiceProvider serviceProvider)
        {
            return this.tokenRequest(serviceProvider);
        }

        /// <summary>
        /// 刷新 token
        /// </summary> 
        /// <param name="serviceProvider">服务提供者</param>
        /// <param name="refresh_token">刷新 token</param>
        /// <returns></returns>
        protected override Task<TokenResult?> RefreshTokenAsync(IServiceProvider serviceProvider, string refresh_token)
        {
            return this.RequestTokenAsync(serviceProvider);
        }
    }
}
