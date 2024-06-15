using System.Threading.Tasks;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 定义 token 提供者的接口
    /// </summary>
    public interface ITokenProvider
    {
        /// <summary>
        /// 设置名称
        /// </summary>
        string Name { set; }

        /// <summary>
        /// 强制清除 token 以支持下次获取到新的 token
        /// </summary>
        void ClearToken();

        /// <summary>
        /// 获取 token 信息
        /// </summary> 
        /// <returns></returns>
        Task<TokenResult> GetTokenAsync();
    }
}