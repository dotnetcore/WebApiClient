using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using WebApiClientCore;
using WebApiClientCore.ResponseCaches;
using WebApiClientCore.Serialization;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 提供HttpApi相关扩展
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

            return services
                .AddHttpClient(typeof(THttpApi).FullName)
                .AddTypedClient((httpClient, serviceProvider) =>
                {
                    var name = typeof(THttpApi).FullName;
                    var httpApiOptions = serviceProvider.GetRequiredService<IOptionsMonitor<HttpApiOptions>>().Get(name);
                    return HttpApi.Create<THttpApi>(httpClient, serviceProvider, httpApiOptions);
                });
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
            var name = typeof(THttpApi).FullName;
            services.AddOptions<HttpApiOptions>(name).Configure(configureOptions);
            return services.AddHttpApi<THttpApi>();
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
            var name = typeof(THttpApi).FullName;
            services.AddOptions<HttpApiOptions>(name).Configure(configureOptions);
            return services.AddHttpApi<THttpApi>();
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

            var builderType = typeof(HttpApiBuilder<>).MakeGenericType(httpApiType);
            return builderType.CreateInstance<IHttpApiBuilder>(services).AddHttpApi();
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
            var name = httpApiType.FullName;
            services.AddOptions<HttpApiOptions>(name).Configure(configureOptions);
            return services.AddHttpApi(httpApiType);
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
            var name = httpApiType.FullName;
            services.AddOptions<HttpApiOptions>(name).Configure(configureOptions);
            return services.AddHttpApi(httpApiType);
        }

        /// <summary>
        /// 定义httpApi的Builder的行为
        /// </summary>
        private interface IHttpApiBuilder
        {
            /// <summary>
            /// 添加HttpApi代理类到服务
            /// </summary>
            /// <returns></returns>
            IHttpClientBuilder AddHttpApi();
        }

        /// <summary>
        /// httpApi的Builder
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        private class HttpApiBuilder<THttpApi> : IHttpApiBuilder where THttpApi : class
        {
            private readonly IServiceCollection services;

            /// <summary>
            /// httpApi的Builder
            /// </summary>
            /// <param name="services"></param>
            public HttpApiBuilder(IServiceCollection services)
            {
                this.services = services;
            }

            /// <summary>
            /// 添加HttpApi代理类到服务
            /// </summary> 
            /// <returns></returns>
            public IHttpClientBuilder AddHttpApi()
            {
                return this.services.AddHttpApi<THttpApi>();
            }
        }
    }
}
