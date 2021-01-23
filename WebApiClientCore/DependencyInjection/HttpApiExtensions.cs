using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using WebApiClientCore;
using WebApiClientCore.Implementations;

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
            var name = HttpApi.GetName(typeof(THttpApi));

            services.AddWebApiClient();
            services.NamedHttpApiType(name, typeof(THttpApi));
            services.TryAddSingleton(typeof(HttpApiProvider<>));
            services.TryAddTransient(serviceProvider =>
            {
                var httiApiProvider = serviceProvider.GetRequiredService<HttpApiProvider<THttpApi>>();
                return httiApiProvider.CreateHttpApi(serviceProvider, name);
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
        /// 表示THttpApi提供者
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        private class HttpApiProvider<THttpApi>
        {
            private readonly IHttpClientFactory httpClientFactory;
            private readonly IOptionsMonitor<HttpApiOptions> httpApiOptionsMonitor;
            private readonly IHttpApiActivator<THttpApi> httpApiActivator;

            /// <summary>
            /// THttpApi提供者
            /// </summary>
            /// <param name="httpClientFactory"></param>
            /// <param name="httpApiOptionsMonitor"></param>
            /// <param name="httpApiActivator"></param>
            public HttpApiProvider(IHttpClientFactory httpClientFactory, IOptionsMonitor<HttpApiOptions> httpApiOptionsMonitor, IHttpApiActivator<THttpApi> httpApiActivator)
            {
                this.httpClientFactory = httpClientFactory;
                this.httpApiOptionsMonitor = httpApiOptionsMonitor;
                this.httpApiActivator = httpApiActivator;
            }

            /// <summary>
            /// 创建THttpApi的实例
            /// </summary>
            /// <param name="serviceProvider">服务提供者</param>
            /// <param name="name">名称</param>
            /// <returns></returns>
            public THttpApi CreateHttpApi(IServiceProvider serviceProvider, string name)
            {
                var httpClient = this.httpClientFactory.CreateClient(name);
                var httpApiOptions = this.httpApiOptionsMonitor.Get(name);

                var httpClientContext = new HttpClientContext(httpClient, serviceProvider, httpApiOptions, name);
                var httpApiInterceptor = new HttpApiInterceptor(httpClientContext);

                return this.httpApiActivator.CreateInstance(httpApiInterceptor);
            }
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
