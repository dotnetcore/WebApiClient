using System;
using System.Collections.Concurrent;

namespace WebApiClient
{
    /// <summary>
    /// 提供HttpApi的注册和实例创建
    /// 使用HttpApiFactory自动管理实例的生命周期
    /// </summary>
    public partial class HttpApi
    {
        /// <summary>
        /// 同步锁
        /// </summary>
        private static readonly object syncRoot = new object();

        /// <summary>
        /// 工厂字典
        /// </summary>
        private static readonly ConcurrentDictionary<string, IHttpApiFactory> factories = new ConcurrentDictionary<string, IHttpApiFactory>();


        /// <summary>
        /// 注册指定Api以及其工厂
        /// 返回Api工厂实例
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static HttpApiFactory<TInterface> Register<TInterface>() where TInterface : class, IHttpApi
        {
            var name = GetFactoryName<TInterface>();
            return Register<TInterface>(name);
        }

        /// <summary>
        /// 注册指定Api以及其工厂
        /// 返回Api工厂实例
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="name">工厂名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static HttpApiFactory<TInterface> Register<TInterface>(string name) where TInterface : class, IHttpApi
        {
            var factory = new HttpApiFactory<TInterface>();
            return Register(name, factory);
        }

        /// <summary>
        /// 注册指定Api以及其工厂
        /// 返回Api工厂实例
        /// </summary>
        /// <param name="name">工厂名称</param>
        /// <param name="interfaceType">api接口类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static HttpApiFactory Register(string name, Type interfaceType)
        {
            var factory = new HttpApiFactory(interfaceType);
            return Register(name, factory);
        }

        /// <summary>
        /// 注册指定Api的工厂
        /// </summary>
        /// <typeparam name="TFactory"></typeparam>
        /// <param name="name">工厂名称</param>
        /// <param name="factory">工厂实例</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        private static TFactory Register<TFactory>(string name, TFactory factory) where TFactory : IHttpApiFactory
        {
            if (string.IsNullOrEmpty(name) == true)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            lock (syncRoot)
            {
                if (factories.ContainsKey(name) == true)
                {
                    throw new InvalidOperationException($"不允许添加重复名称的{nameof(HttpApiFactory)}：{name}");
                }

                factories.TryAdd(name, factory);
                return factory;
            }
        }

        /// <summary>
        /// 获取指定Api的代理实例
        /// 不能将该实例作全局变量引用
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static TInterface Get<TInterface>() where TInterface : class, IHttpApi
        {
            var name = GetFactoryName<TInterface>();
            return Get<TInterface>(name);
        }

        /// <summary>
        /// 根据工厂名获取其Api的代理实例
        /// 不能将该实例作全局变量引用
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="name">工厂名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static TInterface Get<TInterface>(string name) where TInterface : class, IHttpApi
        {
            return Get(name) as TInterface;
        }

        /// <summary>
        /// 根据工厂名获取其Api的代理实例
        /// 不能将该实例作全局变量引用
        /// </summary>
        /// <param name="name">工厂名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static HttpApi Get(string name)
        {
            if (string.IsNullOrEmpty(name) == true)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (factories.TryGetValue(name, out var factory) == true)
            {
                return factory.CreateHttpApi();
            }
            throw new InvalidOperationException($"尚未Register{nameof(name)}为{name}的接口");
        }

        /// <summary>
        /// 返回类型的工厂名称
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <returns></returns>
        private static string GetFactoryName<TInterface>()
        {
            return typeof(TInterface).FullName;
        }
    }
}
