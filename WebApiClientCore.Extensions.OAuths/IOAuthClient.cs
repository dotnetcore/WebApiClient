using System;
using System.Threading.Tasks;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 定义Token客户端的接口
    /// </summary>
    public interface IOAuthClient
    {
        /// <summary>
        /// 以client_credentials授权方式获取token
        /// </summary>
        /// <param name="endpoint">token请求地址</param>
        /// <param name="credentials">身份信息</param>
        /// <returns></returns>
        Task<TokenResult?> RequestTokenAsync(Uri endpoint, ClientCredentials credentials);

        /// <summary>
        /// 以password授权方式获取token
        /// </summary>
        /// <param name="endpoint">token请求地址</param>
        /// <param name="credentials">身份信息</param>
        /// <returns></returns>
        Task<TokenResult?> RequestTokenAsync(Uri endpoint, PasswordCredentials credentials);

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="endpoint">token请求地址</param>
        /// <param name="credentials">身份信息</param>
        /// <returns></returns>
        Task<TokenResult?> RefreshTokenAsync(Uri endpoint, RefreshTokenCredentials credentials);
    }
}
