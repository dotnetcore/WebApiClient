using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 定义多应用下token提供者的接口
    /// </summary>
    public interface IDynamicTokenProvider : ITokenProvider
    {
        /// <summary>
        /// 强制清除token以支持下次获取到新的token
        /// </summary>
        /// <param name="identifier">应用标识</param>
        Task ClearTokenAsync(string identifier);

        /// <summary>
        /// 根据应用标识获取token信息
        /// </summary>
        /// <param name="identifier">应用标识</param>
        Task<TokenResult> GetTokenAsync(string identifier);
    }
}
