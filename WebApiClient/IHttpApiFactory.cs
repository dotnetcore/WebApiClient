using System;
using System.Net.Http;

namespace WebApiClient
{
    /// <summary>
    /// 定义HttpApiFactory的接口
    /// </summary>
    public interface IHttpApiFactory
    {
        /// <summary>
        /// 创建接口的代理实例
        /// </summary>
        /// <returns></returns>
        HttpApiClient CreateHttpApi();

        /// <summary>
        /// 配置HttpApiConfig
        /// </summary>
        /// <param name="options">配置委托</param>
        void ConfigureHttpApiConfig(Action<HttpApiConfig> options);

        /// <summary>
        /// 配置HttpMessageHandler的创建
        /// </summary>
        /// <param name="factory">创建委托</param>
        void ConfigureHttpMessageHandler(Func<HttpMessageHandler> factory);
    }

    /// <summary>
    /// 定义HttpApi工厂的接口
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    public interface IHttpApiFactory<TInterface> : IHttpApiFactory where TInterface : class, IHttpApi
    {
        /// <summary>
        /// 创建接口的代理实例
        /// </summary>
        /// <returns></returns>
        new TInterface CreateHttpApi();
    }
}
