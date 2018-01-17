using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient.Interfaces
{
    /// <summary>
    /// 定义HttpApi客户端的接口
    /// </summary>
    public interface IHttpApiClient : IDisposable
    {
        /// <summary>
        /// 获取相关配置
        /// </summary>
        HttpApiConfig ApiConfig { get; }

        /// <summary>
        /// 获取拦截器
        /// </summary>
        IApiInterceptor ApiInterceptor { get; }
    }
}
