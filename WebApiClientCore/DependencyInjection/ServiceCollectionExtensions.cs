using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using WebApiClientCore;
using WebApiClientCore.Defaults;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 提供项目相关扩展
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加HttpApi代理类到服务
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddHttpApi<THttpApi>(this IServiceCollection services) where THttpApi : class, IHttpApi
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
        public static IHttpClientBuilder AddHttpApi<THttpApi>(this IServiceCollection services, Action<HttpApiOptions<THttpApi>> configureOptions) where THttpApi : class, IHttpApi
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
        public static IHttpClientBuilder AddHttpApi<THttpApi>(this IServiceCollection services, Action<HttpApiOptions<THttpApi>, IServiceProvider> configureOptions) where THttpApi : class, IHttpApi
        {
            services
                .AddHttpDefaults()
                .AddOptions<HttpApiOptions<THttpApi>>()
                .Configure(configureOptions);

            return services
                .AddHttpClient(typeof(THttpApi).FullName)
                .AddTypedClient((client, serviceProvider) =>
                {
                    var options = serviceProvider.GetService<IOptions<HttpApiOptions<THttpApi>>>();
                    return HtttApi.Create<THttpApi>(client, serviceProvider, options.Value);
                });
        }

        /// <summary>
        /// 注册默认组件
        /// </summary>
        /// <param name="services"></param>
        private static IServiceCollection AddHttpDefaults(this IServiceCollection services)
        {
            services.TryAddSingleton<IXmlFormatter, XmlFormatter>();
            services.TryAddSingleton<IJsonFormatter, JsonFormatter>();
            services.TryAddSingleton<IKeyValueFormatter, KeyValueFormatter>();
            services.TryAddSingleton<IResponseCacheProvider, ResponseCacheProvider>();
            return services;
        }

        /// <summary>
        /// 配置HttpApi
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureHttpApi<THttpApi>(this IServiceCollection services, Action<HttpApiOptions<THttpApi>> configureOptions) where THttpApi : class, IHttpApi
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
        public static IServiceCollection ConfigureHttpApi<THttpApi>(this IServiceCollection services, IConfiguration configureOptions) where THttpApi : class, IHttpApi
        {
            return services.Configure<HttpApiOptions<THttpApi>>(configureOptions);
        }
    }
}
