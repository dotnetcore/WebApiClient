using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;

namespace WebApiClient
{
    /// <summary>
    /// HttpHandler提供者
    /// </summary>
    static class HttpHandlerProvider
    {
        /// <summary>
        /// HttpMessageInvoker的HttpMessageHandler字段获取委托
        /// </summary>
        private static readonly Func<HttpMessageInvoker, HttpMessageHandler> handlerGetFunc;

        /// <summary>
        /// 程序集版本信息
        /// </summary>
        private static readonly AssemblyName assemblyName = typeof(HttpHandlerProvider).GetTypeInfo().Assembly.GetName();

        /// <summary>
        /// 默认的UserAgent
        /// </summary>
        public static readonly ProductInfoHeaderValue DefaultUserAgent = new ProductInfoHeaderValue(assemblyName.Name, assemblyName.Version.ToString());

        /// <summary>
        /// HttpHandler提供者
        /// </summary>
        static HttpHandlerProvider()
        {
            var handlerField = typeof(HttpMessageInvoker)
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(field => field.FieldType.IsInheritFrom<HttpMessageHandler>());

            if (handlerField != null)
            {
                handlerGetFunc = Lambda.CreateGetFunc<HttpMessageInvoker, HttpMessageHandler>(handlerField);
            }
        }

        /// <summary>
        /// 从HttpClient获得IHttpHandler
        /// </summary>
        /// <param name="httpClient">httpClient</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public static IHttpHandler CreateHandler(HttpClient httpClient)
        {
            if (handlerGetFunc == null)
            {
                throw new NotSupportedException("无法从HttpClient反射出必须的HttpMessageHandler字段");
            }

            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

            var handler = handlerGetFunc.Invoke(httpClient);
            return CreateHandler(handler);
        }

        /// <summary>
        /// 从HttpMessageHandler获得IHttpHandler包装
        /// </summary>
        /// <param name="handler">HttpMessageHandler实例</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static IHttpHandler CreateHandler(HttpMessageHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            return CreateFrom(handler, handler);
        }

        /// <summary>
        /// 获取HttpMessageHandler关联的HttpClientHandler或SocketsHttpHandler
        /// 返回其IHttpHandler包装
        /// </summary>       
        /// <param name="innerHandler">当前Handler</param>
        /// <param name="sourceHandler">原始的Handler</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        private static IHttpHandler CreateFrom(HttpMessageHandler innerHandler, HttpMessageHandler sourceHandler)
        {
            if (innerHandler is HttpClientHandler clientHandler)
            {
                return new HttpHandlerOfHttpClientHandler(clientHandler, sourceHandler);
            }

#if NETCOREAPP2_1
            if (innerHandler is SocketsHttpHandler socketsHandler)
            {
                return new HttpHandlerOfSocketsHttpHandler(socketsHandler, sourceHandler);
            }
#endif

            if (innerHandler is DelegatingHandler delegatingHandler)
            {
                return CreateFrom(delegatingHandler.InnerHandler, sourceHandler);
            }

            var message = "参数必须为HttpClientHandler、SocketsHttpHandler或DelegatingHandler类型";
            throw new ArgumentException(message, nameof(sourceHandler));
        }
    }
}
