using System;
using System.Net.Http;

namespace WebApiClientCore
{
    /// <summary>
    /// 提供创建HttpApi代理类
    /// </summary>
    public static class HtttApi
    {
        /// <summary>
        /// 创建HttpApi接口的代理类
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="client">httpClient</param>
        /// <param name="services">服务提供者</param>
        /// <param name="options">配置选项</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static THttpApi Create<THttpApi>(HttpClient client, IServiceProvider services, HttpApiOptions options) where THttpApi : IHttpApi
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var context = new ServiceContext(client, services, options);
            var interceptor = new ActionInterceptor(context);
            return HttpApiProxy.CreateInstance<THttpApi>(interceptor);
        }
    }
}
