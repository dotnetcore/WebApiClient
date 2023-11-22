using System.Threading.Tasks;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 定义token提供者的接口
    /// </summary>
    public interface ITokenProvider
    {
        /// <summary>
        /// 设置别名
        /// </summary>
        string Name { set; }

        /// <summary>
        /// 强制清除token以支持下次获取到新的token
        /// </summary>
        void ClearToken();

        /// <summary>
        /// 获取token信息
        /// </summary>
        /// <returns></returns>
        Task<TokenResult> GetTokenAsync();

        /// <summary>
        /// 强制清除token以支持下次获取到新的token
        /// </summary>
        /// <param name="identifier">应用标识</param>
        void ClearToken(string identifier);

        /// <summary>
        /// 根据应用标识获取token信息
        /// </summary>
        /// <param name="identifier">应用标识</param>
        Task<TokenResult> GetTokenAsync(string identifier);
    }
}