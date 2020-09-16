using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using WebApiClientCore;
using WebApiClientCore.ResponseCaches;
using WebApiClientCore.Serialization;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 提供HttpApi注册的扩展
    /// </summary>
    public static class HttpApiExtensions
    {
        /// <summary>
        /// 添加HttpApi代理类到服务
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddHttpApi<THttpApi>(this IServiceCollection services) where THttpApi : class
        {
            services.AddOptions();
            services.AddMemoryCache();
            services.TryAddSingleton<IXmlSerializer, XmlSerializer>();
            services.TryAddSingleton<IJsonSerializer, JsonSerializer>();
            services.TryAddSingleton<IKeyValueSerializer, KeyValueSerializer>();
            services.TryAddSingleton<IResponseCacheProvider, ResponseCacheProvider>();

            var name = HttpApi.GetName(typeof(THttpApi));
            services.NamedHttpApiType(name, typeof(THttpApi));

            services.TryAddTransient(serviceProvider =>
            {
                var httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(name);
                var httpApiOptions = serviceProvider.GetRequiredService<IOptionsMonitor<HttpApiOptions>>().Get(name);
                var httpClientContext = new HttpClientContext(httpClient, serviceProvider, httpApiOptions, name);
                return HttpApi.Create<THttpApi>(httpClientContext);
            });

            return services.AddHttpClient(name);
        }

        /// <summary>
        /// 添加HttpApi代理类到服务
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions">配置选项</param>
        /// <returns></returns>
        public static IHttpClientBuilder AddHttpApi<THttpApi>(this IServiceCollection services, Action<HttpApiOptions> configureOptions) where THttpApi : class
        {
            return services
                .AddHttpApi<THttpApi>()
                .ConfigureHttpApi(configureOptions);
        }

        /// <summary>
        /// 添加HttpApi代理类到服务
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions">配置选项</param>
        /// <returns></returns>
        public static IHttpClientBuilder AddHttpApi<THttpApi>(this IServiceCollection services, Action<HttpApiOptions, IServiceProvider> configureOptions) where THttpApi : class
        {
            return services
                .AddHttpApi<THttpApi>()
                .ConfigureHttpApi(configureOptions);
        }


        /// <summary>
        /// 添加HttpApi代理类到服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="httpApiType">接口类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IHttpClientBuilder AddHttpApi(this IServiceCollection services, Type httpApiType)
        {
            if (httpApiType == null)
            {
                throw new ArgumentNullException(nameof(httpApiType));
            }

            if (httpApiType.IsGenericTypeDefinition == true)
            {
                throw new NotSupportedException(Resx.unsupported_GenericTypeDefinitionType.Format(httpApiType));
            }

            var builderType = typeof(HttpApiBuilder<>).MakeGenericType(httpApiType);
            return builderType.CreateInstance<IHttpApiBuilder>().AddHttpApi(services);
        }

        /// <summary>
        /// 添加HttpApi代理类到服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="httpApiType">接口类型</param>
        /// <param name="configureOptions">配置选项</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IHttpClientBuilder AddHttpApi(this IServiceCollection services, Type httpApiType, Action<HttpApiOptions> configureOptions)
        {
            return services
                .AddHttpApi(httpApiType)
                .ConfigureHttpApi(configureOptions);
        }

        /// <summary>
        /// 添加HttpApi代理类到服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="httpApiType">接口类型</param>
        /// <param name="configureOptions">配置选项</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IHttpClientBuilder AddHttpApi(this IServiceCollection services, Type httpApiType, Action<HttpApiOptions, IServiceProvider> configureOptions)
        {
            return services
                .AddHttpApi(httpApiType)
                .ConfigureHttpApi(configureOptions);
        }


        /// <summary>
        /// 定义httpApi的Builder的行为
        /// </summary>
        private interface IHttpApiBuilder
        {
            /// <summary>
            /// 添加HttpApi代理类到服务
            /// </summary>
            /// <param name="services"></param>
            /// <returns></returns>
            IHttpClientBuilder AddHttpApi(IServiceCollection services);
        }

        /// <summary>
        /// httpApi的Builder
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        private class HttpApiBuilder<THttpApi> : IHttpApiBuilder where THttpApi : class
        {
            /// <summary>
            /// 添加HttpApi代理类到服务
            /// </summary> 
            /// <returns></returns>
            public IHttpClientBuilder AddHttpApi(IServiceCollection services)
            {
                return services.AddHttpApi<THttpApi>();
            }
        }
    }
}
