using System.Threading.Tasks;

namespace WebApiClientCore.Extensions.OAuths.TokenClients
{
    /// <summary>
    /// 表示处理指定接口类型的自定义token客户端抽象类
    /// <typeparam name="THttpApi">处理的接口类型</typeparam>
    /// </summary>
    public abstract class CustomTokenClient<THttpApi> : ICustomTokenClient<THttpApi>, ICustomTokenClient
    {
        /// <summary>
        /// 请求获取token
        /// </summary>
        /// <returns></returns>
        public abstract Task<TokenResult?> RequestTokenAsync();

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="refresh_token">刷新令牌</param>
        /// <returns></returns>
        public virtual Task<TokenResult?> RefreshTokenAsync(string refresh_token)
        {
            return this.RequestTokenAsync();
        }
    }
}
