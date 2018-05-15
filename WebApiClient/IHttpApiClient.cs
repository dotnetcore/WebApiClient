using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 定义HttpApiClient的接口
    /// </summary>
    public interface IHttpApiClient : IHttpApi
    {
        /// <summary>
        /// 获取拦截器
        /// </summary>
        IApiInterceptor ApiInterceptor { get; }
    }
}
