using System;
using WebApiClient.Defaults;

namespace WebApiClient
{
    /// <summary>
    /// Provides creation, registration, and parsing of HttpApi 
    /// </summary>
    public partial class HttpApi
    {
        /// <summary>
        /// Create a proxy instance for the specified interface
        /// </summary>
        /// <typeparam name="TInterface">Interface Type</typeparam>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        /// <returns></returns>
        public static TInterface Create<TInterface>() where TInterface : class, IHttpApi
        {
            var config = new HttpApiConfig();
            return Create<TInterface>(config);
        }

        /// <summary>
        /// Create a proxy instance for the specified interface
        /// </summary>
        /// <typeparam name="TInterface">Interface Type</typeparam>
        /// <param name="httpHost">Http service full host domain name, such as http://www.webapiclient.com</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        /// <returns></returns>
        public static TInterface Create<TInterface>(string httpHost) where TInterface : class, IHttpApi
        {
            var config = new HttpApiConfig();
            if (string.IsNullOrEmpty(httpHost) == false)
            {
                config.HttpHost = new Uri(httpHost, UriKind.Absolute);
            }
            return Create<TInterface>(config);
        }

        /// <summary>
        /// Create a proxy instance for the specified interface
        /// </summary>
        /// <typeparam name="TInterface">Interface Type</typeparam>
        /// <param name="httpApiConfig">Interface configuration</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        /// <returns></returns>
        public static TInterface Create<TInterface>(HttpApiConfig httpApiConfig) where TInterface : class, IHttpApi
        {
            return Create(typeof(TInterface), httpApiConfig) as TInterface;
        }

        /// <summary>
        /// Create a proxy instance for the specified interface
        /// The proxy instance is derived from the HttpApi type
        /// </summary>
        /// <param name="interfaceType">Request interface type</param>
        /// <param name="httpApiConfig">Interface configuration</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        /// <returns></returns>
        public static HttpApi Create(Type interfaceType, HttpApiConfig httpApiConfig)
        {
            if (httpApiConfig == null)
            {
                throw new ArgumentNullException(nameof(httpApiConfig));
            }
            var interceptor = new ApiInterceptor(httpApiConfig);
            return Create(interfaceType, interceptor);
        }

        /// <summary>
        /// Create a proxy instance for the specified interface
        /// The proxy instance is derived from the HttpApi type
        /// </summary>
        /// <param name="interfaceType">Request interface type</param>
        /// <param name="apiInterceptor">http interface call interceptor</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        /// <returns></returns>
        public static HttpApi Create(Type interfaceType, IApiInterceptor apiInterceptor)
        {
            if (interfaceType == null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }

            if (apiInterceptor == null)
            {
                throw new ArgumentNullException(nameof(apiInterceptor));
            }

            return HttpApiProxy.CreateInstance(interfaceType, apiInterceptor);
        }
    }
}
