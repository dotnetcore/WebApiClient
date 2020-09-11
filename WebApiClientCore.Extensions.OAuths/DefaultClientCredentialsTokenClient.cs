using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using WebApiClientCore.Extensions.OAuths.Exceptions;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 表示默认的client授权模式的Token客户端
    /// </summary>
    class DefaultClientCredentialsTokenClient : IClientCredentialsTokenClient
    {
        /// <summary>
        /// OAuth客户端接口
        /// </summary>
        private readonly ITokenClientApi clientApi;

        /// <summary>
        /// 选项
        /// </summary>
        private readonly IOptionsMonitor<ClientCredentialsOptions> optionsMonitor;

        /// <summary>
        /// client授权模式的Token客户端
        /// </summary>
        /// <param name="clientApi">OAuth客户端接口</param>
        /// <param name="optionsMonitor">选项</param> 
        public DefaultClientCredentialsTokenClient(
            ITokenClientApi clientApi,
            IOptionsMonitor<ClientCredentialsOptions> optionsMonitor)
        {
            this.clientApi = clientApi;
            this.optionsMonitor = optionsMonitor;
        }

        /// <summary>
        /// 请求获取token
        /// </summary>
        /// <param name="httpApiType">http接口类型</param> 
        /// <returns></returns>
        public Task<TokenResult?> RequestTokenAsync(Type? httpApiType)
        {
            var name = HttpApi.GetName(httpApiType);
            var options = this.optionsMonitor.Get(name);

            if (options.Endpoint == null)
            {
                throw new TokenEndPointNullException();
            }
            return this.clientApi.RequestTokenAsync(options.Endpoint, options.Credentials);
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="refresh_token">刷新令牌</param>
        /// <param name="httpApiType">http接口类型</param>
        /// <returns></returns>
        public Task<TokenResult?> RefreshTokenAsync(string refresh_token, Type? httpApiType)
        {
            var name = HttpApi.GetName(httpApiType);
            var options = this.optionsMonitor.Get(name);

            if (options.Endpoint == null)
            {
                throw new TokenEndPointNullException();
            }

            if (options.UseRefreshToken == false)
            {
                return this.RequestTokenAsync(httpApiType);
            }

            var refreshCredentials = new RefreshTokenCredentials
            {
                Client_id = options.Credentials.Client_id,
                Client_secret = options.Credentials.Client_secret,
                Extra = options.Credentials.Extra,
                Refresh_token = refresh_token
            };
            return this.clientApi.RefreshTokenAsync(options.Endpoint, refreshCredentials);
        }
    }
}
