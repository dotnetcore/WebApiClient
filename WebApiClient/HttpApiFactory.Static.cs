using System;
using System.Net.Http;

namespace WebApiClient
{
    /// <summary>
    /// 表示HttpApi创建工厂
    /// 提供HttpApi的配置注册和实例创建
    /// 并对实例的生命周期进行自动管理
    /// </summary>
    public partial class HttpApiFactory
    {
        /// <summary>
        /// 获取默认工厂实例
        /// </summary>
        public static readonly HttpApiFactory Default = new HttpApiFactory();

        /// <summary>
        /// 注册http接口到默认工厂
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <returns></returns>
        public static bool Add<TInterface>() where TInterface : class, IHttpApi
        {
            return Add<TInterface>(config: null);
        }

        /// <summary>
        /// 注册http接口到默认工厂
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="config">HttpApiConfig的配置</param>
        /// <returns></returns>
        public static bool Add<TInterface>(Action<HttpApiConfig> config) where TInterface : class, IHttpApi
        {
            return Add<TInterface>(config, handlerFactory: null);
        }

        /// <summary>
        /// 注册http接口到默认工厂
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="config">HttpApiConfig的配置</param>
        /// <param name="handlerFactory">HttpMessageHandler创建委托</param>
        /// <returns></returns>
        public static bool Add<TInterface>(Action<HttpApiConfig> config, Func<HttpMessageHandler> handlerFactory) where TInterface : class, IHttpApi
        {
            return Default.AddHttpApi<TInterface>(config, handlerFactory);
        }

        /// <summary>
        /// 使用默认工厂创建指定接口的代理实例
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static TInterface Create<TInterface>() where TInterface : class, IHttpApi
        {
            return Default.CreateHttpApi<TInterface>();
        }
    }
}
