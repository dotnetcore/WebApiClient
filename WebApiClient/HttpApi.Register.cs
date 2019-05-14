using System;
using System.Collections.Concurrent;

namespace WebApiClient
{
    /// <summary>
    /// 提供HttpApi的创建、注册和解析   
    /// </summary>
    public partial class HttpApi
    {
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
            var factory = new HttpApiFactory<TInterface>();
            return RegisterFactory(factory);
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
            return RegisterFactory(factory, name);
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
            return RegisterFactory(factory, name);
        }

        /// <summary>
        /// 注册指定Api工厂
        /// </summary>
        /// <typeparam name="THttpApiFactory"></typeparam>    
        /// <param name="httpApiFactory">工厂实例</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static THttpApiFactory RegisterFactory<THttpApiFactory>(THttpApiFactory httpApiFactory) where THttpApiFactory : IHttpApiFactory
        {
            var name = GetFactoryName(httpApiFactory.InterfaceType);
            return RegisterFactory(httpApiFactory, name);
        }

        /// <summary>
        /// 注册指定Api工厂
        /// </summary>
        /// <typeparam name="THttpApiFactory"></typeparam>
        /// <param name="httpApiFactory">工厂实例</param>
        /// <param name="name">工厂名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static THttpApiFactory RegisterFactory<THttpApiFactory>(THttpApiFactory httpApiFactory, string name) where THttpApiFactory : IHttpApiFactory
        {
            if (httpApiFactory == null)
            {
                throw new ArgumentNullException(nameof(httpApiFactory));
            }
            if (string.IsNullOrEmpty(name) == true)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (factories.TryAdd(name, httpApiFactory) == true)
            {
                return httpApiFactory;
            }
            throw new InvalidOperationException($"不允许注册重复名称的工厂名称：{name}");
        }

        /// <summary>
        /// 解析出指定Api的代理实例
        /// 不能将该实例作全局变量引用
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static TInterface Resolve<TInterface>() where TInterface : class, IHttpApi
        {
            var name = GetFactoryName(typeof(TInterface));
            return Resolve<TInterface>(name);
        }

        /// <summary>
        /// 根据工厂名解析出其Api的代理实例
        /// 不能将该实例作全局变量引用
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="name">工厂名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static TInterface Resolve<TInterface>(string name) where TInterface : class, IHttpApi
        {
            return Resolve(name) as TInterface;
        }

        /// <summary>
        /// 根据工厂名解析出其Api的代理实例
        /// 不能将该实例作全局变量引用
        /// </summary>
        /// <param name="name">工厂名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static HttpApi Resolve(string name)
        {
            if (string.IsNullOrEmpty(name) == true)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (factories.TryGetValue(name, out var factory) == true)
            {
                return factory.CreateHttpApi();
            }
            throw new InvalidOperationException($"尚未Register(){nameof(name)}为{name}的接口");
        }

        /// <summary>
        /// 返回类型的工厂名称
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <returns></returns>
        private static string GetFactoryName(Type interfaceType)
        {
            return interfaceType.FullName;
        }
    }
}
