using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;

namespace WebApiClientCore.Extensions.OAuths.TokenProviders
{
    /// <summary>
    /// 定义OAuth2的Token客户端的接口
    /// </summary>    
    [LoggingFilter]
    [XmlReturn(Enable = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [JsonReturn(EnsureMatchAcceptContentType = false, EnsureSuccessStatusCode = false)]
    public interface IOAuthTokenClientApi
    {
        /// <summary>
        /// 以client_credentials授权方式获取token
        /// </summary>
        /// <param name="endpoint">token请求地址</param>
        /// <param name="credentials">身份信息</param>
        /// <returns></returns>
        [HttpPost]
        [FormField("grant_type", "client_credentials")]
        Task<TokenResult?> RequestTokenAsync([Required, Uri] Uri endpoint, [Required, FormContent] ClientCredentials credentials);

        /// <summary>
        /// 以password授权方式获取token
        /// </summary>
        /// <param name="endpoint">token请求地址</param>
        /// <param name="credentials">身份信息</param>
        /// <returns></returns>
        [HttpPost]
        [FormField("grant_type", "password")]
        Task<TokenResult?> RequestTokenAsync([Required, Uri] Uri endpoint, [Required, FormContent] PasswordCredentials credentials);

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="endpoint">token请求地址</param>
        /// <param name="credentials">身份信息</param>
        /// <returns></returns>
        [HttpPost]
        [FormField("grant_type", "refresh_token")]
        Task<TokenResult?> RefreshTokenAsync([Required, Uri] Uri endpoint, [Required, FormContent] RefreshTokenCredentials credentials);
    }
}
