using System;
using System.Net.Http;

namespace WebApiClient.Defaults
{
    /// <summary>
    /// 提供IHttpHandler对象的获取
    /// </summary>
    public static class HttpHandler
    {
        /// <summary>
        /// 从HttpClientHandler获得IHttpHandler包装
        /// </summary>
        /// <param name="handler">HttpClientHandler实例</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IHttpHandler From(HttpClientHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            return new HttpHandlerOfHttpClientHandler(handler);
        }

#if NETCOREAPP2_1
        /// <summary>
        /// 从SocketsHttpHandler获得IHttpHandler包装
        /// </summary>
        /// <param name="handler">SocketsHttpHandler实例</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IHttpHandler From(SocketsHttpHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            return new HttpHandlerOfSocketsHttpHandler(handler);
        }
#endif

        /// <summary>
        /// 从HttpMessageHandler获得IHttpHandler包装
        /// </summary>
        /// <param name="handler">HttpMessageHandler实例</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IHttpHandler From(HttpMessageHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            return HttpHandler.FromHttpMessageHandler(handler, handler);
        }

        /// <summary>
        /// 获取HttpMessageHandler关联的HttpClientHandler或SocketsHttpHandler
        /// 返回其IHttpHandler包装
        /// </summary>       
        /// <param name="handler">当前Handler</param>
        /// <param name="sourceHandler">原始的Handler</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        private static IHttpHandler FromHttpMessageHandler(HttpMessageHandler handler, HttpMessageHandler sourceHandler)
        {

#if NETCOREAPP2_1
            if (handler is SocketsHttpHandler socketsHandler)
            {
                return new HttpHandlerOfSocketsHttpHandler(socketsHandler, sourceHandler);
            }
#endif
            if (handler is HttpClientHandler clientHandler)
            {
                return new HttpHandlerOfHttpClientHandler(clientHandler, sourceHandler);
            }

            if (handler is DelegatingHandler delegatingHandler)
            {
                return HttpHandler.FromHttpMessageHandler(delegatingHandler.InnerHandler, sourceHandler);
            }

            var message = "参数必须为HttpClientHandler、SocketsHttpHandler或DelegatingHandler类型";
            throw new ArgumentException(message, nameof(sourceHandler));
        }

        /// <summary>
        /// 创建默认的IHanlder
        /// .net core2.1或以上使用SocketsHttpHandler
        /// </summary>
        /// <returns></returns>
        public static IHttpHandler CreateHanlder()
        {
#if NETCOREAPP2_1
            var set = AppContext.TryGetSwitch("System.Net.Http.UseSocketsHttpHandler", out bool enable);
            if (set && enable == false)
            {
                return new DefaultHttpClientHandler();
            }

            var handler = new SocketsHttpHandler
            {
                UseProxy = false,
                Proxy = null,
                MaxConnectionsPerServer = HttpApiClient.ConnectionLimit
            };
            handler.SslOptions.RemoteCertificateValidationCallback = (a, b, c, d) => true;
            return HttpHandler.From(handler);
#else
            return new DefaultHttpClientHandler();
#endif
        }
    }
}
