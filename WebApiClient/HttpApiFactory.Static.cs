using System;
using System.Collections.Concurrent;
using System.Net.Http;

namespace WebApiClient
{
    /// <summary>
    /// 表示HttpApi创建工厂表示操作
    /// 提供HttpApi的配置注册和实例创建
    /// 并对实例的生命周期进行自动管理
    /// </summary>
    public static class HttpApiFactory
    {
        /// <summary>
        /// 同步锁
        /// </summary>
        private static object syncRoot = new object();

        /// <summary>
        /// 工厂字典
        /// </summary>
        private static readonly ConcurrentDictionary<Type, IHttpApiFactory> factories;

        /// <summary>
        /// 表示HttpApi创建工厂
        /// </summary>
        static HttpApiFactory()
        {
            factories = new ConcurrentDictionary<Type, IHttpApiFactory>();
        }

        /// <summary>
        /// 注册http接口
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <returns></returns>
        public static bool Add<TInterface>() where TInterface : class, IHttpApi
        {
            return Add<TInterface>(configAction: null);
        }

        /// <summary>
        /// 注册http接口
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="configAction">HttpApiConfig的配置委托</param>
        /// <returns></returns>
        public static bool Add<TInterface>(Action<HttpApiConfig> configAction) where TInterface : class, IHttpApi
        {
            return Add<TInterface>(configAction, handlerFunc: null);
        }

        /// <summary>
        /// 注册http接口
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="configAction">HttpApiConfig的配置</param>
        /// <param name="handlerFunc">HttpMessageHandler创建委托</param>
        /// <returns></returns>
        public static bool Add<TInterface>(Action<HttpApiConfig> configAction, Func<HttpMessageHandler> handlerFunc) where TInterface : class, IHttpApi
        {
            lock (syncRoot)
            {
                var apiType = typeof(TInterface);
                if (factories.ContainsKey(apiType) == true)
                {
                    return false;
                }

                var factory = new HttpApiFactory<TInterface>(configAction, handlerFunc);
                return factories.TryAdd(apiType, factory);
            }
        }

        /// <summary>
        /// 创建指定接口的代理实例
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static TInterface Create<TInterface>() where TInterface : class, IHttpApi
        {
            if (factories.TryGetValue(typeof(TInterface), out var factory) == true)
            {
                return factory.CreateHttpApi() as TInterface;
            }
            throw new InvalidOperationException($"未注册的接口类型：{typeof(TInterface)}");
        }
    }
}
