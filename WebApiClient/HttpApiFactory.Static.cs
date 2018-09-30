using System;
using System.Collections.Concurrent;

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
        private static readonly ConcurrentDictionary<Type, _IHttpApiFactory> factories;

        /// <summary>
        /// 表示HttpApi创建工厂
        /// </summary>
        static HttpApiFactory()
        {
            factories = new ConcurrentDictionary<Type, _IHttpApiFactory>();
        }

        /// <summary>
        /// 创建并记录指定接口的HttpApiFactory
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static HttpApiFactory<TInterface> Add<TInterface>() where TInterface : class, IHttpApi
        {
            lock (syncRoot)
            {
                var apiType = typeof(TInterface);
                if (factories.ContainsKey(apiType) == true)
                {
                    throw new InvalidOperationException($"不允许重复注册接口：{apiType}");
                }

                var factory = new HttpApiFactory<TInterface>();
                factories.TryAdd(apiType, factory);
                return factory;
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
