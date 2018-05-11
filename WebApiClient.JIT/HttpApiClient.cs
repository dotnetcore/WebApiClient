using System;
using WebApiClient.Defaults;

namespace WebApiClient
{
    /// <summary>
    /// 表示HttpApi客户端
    /// 提供创建HttpApiClient实例的方法
    /// </summary>
    public static class HttpApiClient
    {
        /// <summary>
        /// 获取或设置一个站点内的默认连接数限制
        /// 这个值在初始化HttpClientHandler时使用
        /// 默认值为128
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int ConnectionLimit
        {
            get => HttpClientOptions.ConnectionLimit;
            set => HttpClientOptions.ConnectionLimit = value;
        }

        /// <summary>
        /// 创建实现了指定接口的HttpApiClient实例
        /// </summary>
        /// <typeparam name="TInterface">请求接口类型</typeparam>
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
        /// 创建实现了指定接口的HttpApiClient实例
        /// </summary>
        /// <typeparam name="TInterface">请求接口类型</typeparam>
        /// <param name="httpHost">Http服务完整主机域名，如http://www.webapiclient.com</param>
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
        /// 创建实现了指定接口的HttpApiClient实例
        /// </summary>
        /// <typeparam name="TInterface">请求接口类型</typeparam>
        /// <param name="httpApiConfig">接口配置</param>
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
        /// 创建实现了指定接口的HttpApiClient实例
        /// </summary>
        /// <param name="interfaceType">请求接口类型</param>
        /// <param name="httpApiConfig">接口配置</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        /// <returns></returns>
        public static object Create(Type interfaceType, HttpApiConfig httpApiConfig)
        {
            if (httpApiConfig == null)
            {
                throw new ArgumentNullException(nameof(httpApiConfig));
            }
            var interceptor = new ApiInterceptor(httpApiConfig);
            return Create(interfaceType, interceptor);
        }

        /// <summary>
        /// 创建实现了指定接口的HttpApiClient实例
        /// </summary>
        /// <param name="interfaceType">请求接口类型</param>
        /// <param name="apiInterceptor">http接口调用拦截器</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        /// <returns></returns>
        private static object Create(Type interfaceType, ApiInterceptor apiInterceptor)
        {
            if (interfaceType == null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }

            if (apiInterceptor == null)
            {
                throw new ArgumentNullException(nameof(apiInterceptor));
            }

            return HttpApiClientProxy.CreateProxyWithInterface(interfaceType, apiInterceptor);
        }
    }
}
