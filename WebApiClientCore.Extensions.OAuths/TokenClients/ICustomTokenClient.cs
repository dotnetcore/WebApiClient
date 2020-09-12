using System.Threading.Tasks;

namespace WebApiClientCore.Extensions.OAuths.TokenClients
{
    /// <summary>
    /// 自定义token客户端
    /// </summary>
    interface ICustomTokenClient
    {
        /// <summary>
        /// 请求获取token
        /// </summary>
        /// <returns></returns>
        Task<TokenResult?> RequestTokenAsync();

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="refresh_token">刷新令牌</param>
        /// <returns></returns>
        Task<TokenResult?> RefreshTokenAsync(string refresh_token);
    }
}
