using System;

namespace WebApiClient
{
    /// <summary>
    /// 定义HttpApi工厂的接口
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    public interface IHttpApiFactory<TInterface> where TInterface : class, IHttpApi
    {
        /// <summary>
        /// 创建接口的代理实例
        /// </summary>
        /// <returns></returns>
        TInterface CreateHttpApi();

        /// <summary>
        /// 配置HttpApiConfig
        /// </summary>
        /// <param name="configAction">配置委托</param>
        void ConfigureHttpApiConfig(Action<HttpApiConfig> configAction);
    }
}
