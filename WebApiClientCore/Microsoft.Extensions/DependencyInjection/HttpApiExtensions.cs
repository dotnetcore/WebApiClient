using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using WebApiClientCore;
using WebApiClientCore.Defaults;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 提供HttpApi相关扩展
    /// </summary>
    public static class HttpApiExtensions
    {
        /// <summary>
        /// 尝试注册默认组件
        /// </summary>
        /// <param name="services"></param>
        private static IServiceCollection AddHttpApi(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.TryAddSingleton<IXmlFormatter, XmlFormatter>();
            services.TryAddSingleton<IJsonFormatter, JsonFormatter>();
            services.TryAddSingleton<IKeyValueFormatter, KeyValueFormatter>();
            services.TryAddSingleton<IResponseCacheProvider, ResponseCacheProvider>();
            return services;
        }

        /// <summary>
        /// 添加HttpApi代理类到服务
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddHttpApi<THttpApi>(this IServiceCollection services) where THttpApi : class
        {
            return services.AddHttpApi<THttpApi>((o, s) => { });
        }

        /// <summary>
        /// 添加HttpApi代理类到服务
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddHttpApi<THttpApi>(this IServiceCollection services, Action<HttpApiOptions<THttpApi>> configureOptions) where THttpApi : class
        {
            return services.AddHttpApi<THttpApi>((o, s) => configureOptions(o));
        }

        /// <summary>
        /// 添加HttpApi代理类到服务
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddHttpApi<THttpApi>(this IServiceCollection services, Action<HttpApiOptions<THttpApi>, IServiceProvider> configureOptions) where THttpApi : class
        {
            services
                .AddHttpApi()
                .AddOptions<HttpApiOptions<THttpApi>>()
                .Configure(configureOptions);

            return services
                .AddHttpClient(typeof(THttpApi).FullName)
                .AddTypedClient((client, serviceProvider) =>
                {
                    var options = serviceProvider.GetService<IOptions<HttpApiOptions<THttpApi>>>();
                    return HttpApi.Create<THttpApi>(client, serviceProvider, options.Value);
                });
        }


        /// <summary>
        /// 配置HttpApi
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureHttpApi<THttpApi>(this IServiceCollection services, Action<HttpApiOptions<THttpApi>> configureOptions)
        {
            return services.Configure(configureOptions);
        }

        /// <summary>
        /// 配置HttpApi
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureHttpApi<THttpApi>(this IServiceCollection services, IConfiguration configureOptions)
        {
            return services.Configure<HttpApiOptions<THttpApi>>(configureOptions);
        }
    }
}
