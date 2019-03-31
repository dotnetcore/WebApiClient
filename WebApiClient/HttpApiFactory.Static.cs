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
        private static readonly object syncRoot = new object();

        /// <summary>
        /// 工厂字典
        /// </summary>
        private static readonly ConcurrentDictionary<string, IHttpApiFactory> factories;

        /// <summary>
        /// 表示HttpApi创建工厂
        /// </summary>
        static HttpApiFactory()
        {
            factories = new ConcurrentDictionary<string, IHttpApiFactory>();
        }

        /// <summary>
        /// 创建并返回指定接口的HttpApiFactory
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static HttpApiFactory<TInterface> Add<TInterface>() where TInterface : class, IHttpApi
        {
            var name = GetFactoryName(typeof(TInterface));
            return Add<TInterface>(name);
        }

        /// <summary>
        /// 创建并返回指定接口的HttpApiFactory
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="name">工厂名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static HttpApiFactory<TInterface> Add<TInterface>(string name) where TInterface : class, IHttpApi
        {
            return Add(name, typeof(TInterface)) as HttpApiFactory<TInterface>;
        }

        /// <summary>
        /// 添加指定接口的HttpApiFactory
        /// </summary>
        /// <param name="name">工厂名称</param>
        /// <param name="interfaceType">api接口类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static IHttpApiFactory Add(string name, Type interfaceType)
        {
            if (string.IsNullOrEmpty(name) == true)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (interfaceType == null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }

            lock (syncRoot)
            {
                if (factories.ContainsKey(name) == true)
                {
                    throw new InvalidOperationException($"不允许添加重复名称的HttpApiFactory：{name}");
                }

                if (interfaceType.IsInheritFrom<IHttpApi>() == false)
                {
                    throw new ArgumentException($"接口类型必须继承{nameof(IHttpApi)}", nameof(interfaceType));
                }

                var factoryType = typeof(HttpApiFactory<>).MakeGenericType(interfaceType);
                var factory = Activator.CreateInstance(factoryType) as IHttpApiFactory;
                factories.TryAdd(name, factory);
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
            var name = GetFactoryName(typeof(TInterface));
            return Create<TInterface>(name);
        }

        /// <summary>
        /// 创建指定接口的代理实例
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="name">工厂名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static TInterface Create<TInterface>(string name) where TInterface : class, IHttpApi
        {
            return Create(name) as TInterface;
        }

        /// <summary>
        /// 创建指定接口的代理实例
        /// </summary>
        /// <param name="name">工厂名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static HttpApiClient Create(string name)
        {
            if (string.IsNullOrEmpty(name) == true)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (factories.TryGetValue(name, out var factory) == true)
            {
                return factory.CreateHttpApi();
            }
            throw new InvalidOperationException($"请先调用HttpApiFactory.Add()方法配置{name}的接口名");
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
