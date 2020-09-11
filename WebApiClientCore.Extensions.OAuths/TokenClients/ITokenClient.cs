using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace WebApiClientCore.Extensions.OAuths.TokenClients
{
    /// <summary>
    /// 定义Token客户端接口
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface ITokenClient
    {
        /// <summary>
        /// 请求获取token
        /// </summary>
        /// <param name="httpApiType">http接口类型</param> 
        /// <returns></returns>
        Task<TokenResult?> RequestTokenAsync(Type? httpApiType);

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="refresh_token">刷新令牌</param>
        /// <param name="httpApiType">http接口类型</param>
        /// <returns></returns>
        Task<TokenResult?> RefreshTokenAsync(string refresh_token, Type? httpApiType);
    }
}
