using System;
using System.Net.Http;

namespace WebApiClient
{
    /// <summary>
    /// 表示HttpApiClient创建工厂
    /// </summary>
    public partial class HttpApiClientFactory
    {
        /// <summary>
        /// 获取默认工厂实例
        /// </summary>
        public static readonly HttpApiClientFactory Default = new HttpApiClientFactory();

        /// <summary>
        /// 注册HttpApiClient对应的http接口到默认工厂
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <exception cref="InvalidOperationException"></exception>
        public static void Add<TInterface>() where TInterface : class, IHttpApi
        {
            Add<TInterface>(config: null);
        }

        /// <summary>
        /// 注册HttpApiClient对应的http接口到默认工厂
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="config">HttpApiConfig的配置</param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void Add<TInterface>(Action<HttpApiConfig> config) where TInterface : class, IHttpApi
        {
            Add<TInterface>(config, handlerFactory: null);
        }

        /// <summary>
        /// 注册HttpApiClient对应的http接口到默认工厂
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="config">HttpApiConfig的配置</param>
        /// <param name="handlerFactory">HttpMessageHandler创建委托</param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void Add<TInterface>(Action<HttpApiConfig> config, Func<HttpMessageHandler> handlerFactory) where TInterface : class, IHttpApi
        {
            Default.AddTypedClient<TInterface>(config, handlerFactory);
        }

        /// <summary>
        /// 使用默认工厂创建实现了指定接口的HttpApiClient实例
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <returns></returns>
        public static TInterface Create<TInterface>() where TInterface : class, IHttpApi
        {
            return Default.CreateTypedClient<TInterface>();
        }
    }
}
