using System;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;

namespace WebApiClientCore.Extensions.OAuths
{

    /// <summary>
    /// 表示默认的Token客户端
    /// </summary>
    class DefaultOAuthClient : IOAuthClient
    {
        /// <summary>
        /// OAuth客户端接口
        /// </summary>
        private readonly IOAuthClientApi clientApi;

        /// <summary>
        /// 默认的Token客户端
        /// </summary>
        /// <param name="clientApi">OAuth客户端接口</param>
        public DefaultOAuthClient(IOAuthClientApi clientApi)
        {
            this.clientApi = clientApi;
        }

        /// <summary>
        /// 以client_credentials授权方式获取token
        /// </summary>
        /// <param name="endpoint">token请求地址</param>
        /// <param name="credentials">身份信息</param>
        /// <returns></returns> 
        public Task<TokenResult?> RequestTokenAsync(Uri endpoint, ClientCredentials credentials)
        {
            return this.clientApi.RequestTokenAsync(endpoint, credentials);
        }

        /// <summary>
        /// 以password授权方式获取token
        /// </summary>
        /// <param name="endpoint">token请求地址</param>
        /// <param name="credentials">身份信息</param>
        /// <returns></returns>
        [HttpPost]
        [FormField("grant_type", "password")]
        public Task<TokenResult?> RequestTokenAsync(Uri endpoint, PasswordCredentials credentials)
        {
            return this.clientApi.RequestTokenAsync(endpoint, credentials);
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="endpoint">token请求地址</param>
        /// <param name="credentials">身份信息</param>
        /// <returns></returns>
        public Task<TokenResult?> RefreshTokenAsync(Uri endpoint, RefreshTokenCredentials credentials)
        {
            return this.clientApi.RefreshTokenAsync(endpoint, credentials);
        }
    }
}
