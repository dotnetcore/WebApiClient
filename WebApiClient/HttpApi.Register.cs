using System;
using System.Collections.Concurrent;

namespace WebApiClient
{
    /// <summary>
    /// Provides creation, registration, and parsing of HttpApi
    /// </summary>
    public partial class HttpApi
    {
        /// <summary>
        /// Factory dictionary
        /// </summary>
        private static readonly ConcurrentDictionary<string, IHttpApiFactory> factories = new ConcurrentDictionary<string, IHttpApiFactory>();


        /// <summary>
        /// Registration of designated Api and its factory
        /// Return to Api factory instance
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
        /// Registration of designated Api and its factory
        /// Return to Api factory instance
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="name">Factory Name</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static HttpApiFactory<TInterface> Register<TInterface>(string name) where TInterface : class, IHttpApi
        {
            var factory = new HttpApiFactory<TInterface>();
            return RegisterFactory(factory, name);
        }

        /// <summary>
        /// Registration of designated Api and its factory
        /// Return to Api factory instance
        /// </summary>
        /// <param name="name">Factory Name</param>
        /// <param name="interfaceType">api interface type</param>
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
        /// Registered designated Api factory
        /// </summary>
        /// <typeparam name="THttpApiFactory"></typeparam>    
        /// <param name="httpApiFactory">Factory example</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static THttpApiFactory RegisterFactory<THttpApiFactory>(THttpApiFactory httpApiFactory) where THttpApiFactory : IHttpApiFactory
        {
            var name = GetFactoryName(httpApiFactory.InterfaceType);
            return RegisterFactory(httpApiFactory, name);
        }

        /// <summary>
        /// Registered designated Api factory
        /// </summary>
        /// <typeparam name="THttpApiFactory"></typeparam>
        /// <param name="httpApiFactory">Factory example</param>
        /// <param name="name">Factory Name</param>
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
            throw new InvalidOperationException($"Factory name registration is not allowed：{name}");
        }

        /// <summary>
        /// Parse out the proxy instance of the specified Api
        /// Cannot reference this instance as a global variable
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
        /// Parse out the proxy instance of its Api based on the factory name
        /// Cannot reference this instance as a global variable
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="name">Factory Name</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static TInterface Resolve<TInterface>(string name) where TInterface : class, IHttpApi
        {
            return Resolve(name) as TInterface;
        }

        /// <summary>
        /// Parse out the proxy instance of its Api based on the factory name
        /// Cannot reference this instance as a global variable
        /// </summary>
        /// <param name="name">Factory Name</param>
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
            throw new InvalidOperationException($"Not yet registered() {nameof(name)} for {name}interface");
        }

        /// <summary>
        /// Factory name of return type
        /// </summary>
        /// <param name="interfaceType">Interface Type</param>
        /// <returns></returns>
        private static string GetFactoryName(Type interfaceType)
        {
            return interfaceType.FullName;
        }
    }
}
