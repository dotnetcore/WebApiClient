using System;
using System.Net.Http;

namespace WebApiClient
{
    /// <summary>
    /// 表示类型客户端选项 
    /// </summary>
    class TypedClientOption
    {
        /// <summary>
        /// 获取或设置配置项
        /// </summary>
        public Action<HttpApiConfig> ConfigAction { get; set; }

        /// <summary>
        /// 获取或设置Handler的创建工厂
        /// </summary>
        public Func<HttpMessageHandler> HandlerFactory { get; set; }
    }
}
