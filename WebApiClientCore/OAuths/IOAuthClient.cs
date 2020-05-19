using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;

namespace WebApiClientCore.OAuths
{
    /// <summary>
    /// 定义Token客户端的接口
    /// </summary>
    [LoggingFilter]
    [JsonModelResult(EnsureMatchContentType = false, EnsureSuccessStatusCode = false)]
    public interface IOAuthClient
    {
        /// <summary>
        /// 以client_credentials授权方式获取token
        /// </summary>
        /// <param name="endpoint">token请求地址</param>
        /// <param name="credentials">身份信息</param>
        /// <returns></returns>
        [HttpPost]
        [FormField("grant_type", "client_credentials")]
        Task<TokenResult> RequestTokenAsync([Required, Uri] Uri endpoint, [Required, FormContent] ClientCredentials credentials);

        /// <summary>
        /// 以password授权方式获取token
        /// </summary>
        /// <param name="endpoint">token请求地址</param>
        /// <param name="credentials">身份信息</param>
        /// <returns></returns>
        [HttpPost]
        [FormField("grant_type", "password")]
        Task<TokenResult> RequestTokenAsync([Required, Uri] Uri endpoint, [Required, FormContent] PasswordCredentials credentials);

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="endpoint">token请求地址</param>
        /// <param name="credentials">身份信息</param>
        /// <returns></returns>
        [HttpPost]
        [FormField("grant_type", "refresh_token")]
        Task<TokenResult> RefreshTokenAsync([Required, Uri] Uri endpoint, [Required, FormContent] RefreshTokenCredentials credentials);
    }
}
