using System;
using System.Net.Http;

namespace WebApiClient
{
    /// <summary>
    /// 表示HttpApi创建选项 
    /// </summary>
    class HttpApiCreateOption
    {
        /// <summary>
        /// 获取或设置配置委托
        /// </summary>
        public Action<HttpApiConfig> ConfigAction { get; set; }

        /// <summary>
        /// 获取或设置HttpMessageHandler的创建工厂
        /// </summary>
        public Func<HttpMessageHandler> HandlerFactory { get; set; }
    }
}
