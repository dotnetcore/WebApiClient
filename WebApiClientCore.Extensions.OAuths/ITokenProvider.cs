using System.Threading.Tasks;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 定义token提供者的接口
    /// </summary>
    public interface ITokenProvider
    {
        /// <summary>
        /// 获取提供者类型
        /// </summary>
        ProviderType ProviderType { get; }

        /// <summary>
        /// 强制清除token以支持下次获取到新的token
        /// </summary>
        void ClearToken();

        /// <summary>
        /// 获取token信息
        /// </summary> 
        /// <returns></returns>
        Task<TokenResult> GetTokenAsync();
    }
}