using Microsoft.Extensions.Configuration;
using System;
using WebApiClientCore;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 提供HttpApi非泛型相关扩展
    /// </summary>
    public static class HttpApiNoGenericExtensions
    {
        /// <summary>
        /// 添加HttpApi代理类到服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="httpApiType"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IHttpClientBuilder AddHttpApi(this IServiceCollection services, Type httpApiType)
        {
            return services.AddHttpApi(httpApiType, (o, s) => { });
        }

        /// <summary>
        /// 添加HttpApi代理类到服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="httpApiType"></param>
        /// <param name="configureOptions"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IHttpClientBuilder AddHttpApi(this IServiceCollection services, Type httpApiType, Action<HttpApiOptions> configureOptions)
        {
            return services.AddHttpApi(httpApiType, (o, s) => configureOptions(o));
        }

        /// <summary>
        /// 添加HttpApi代理类到服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="httpApiType"></param>
        /// <param name="configureOptions"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IHttpClientBuilder AddHttpApi(this IServiceCollection services, Type httpApiType, Action<HttpApiOptions, IServiceProvider> configureOptions)
        {
            var builder = services.CreateHttpApiBuilder(httpApiType);
            return builder.AddHttpApi(configureOptions);
        }

        /// <summary>
        /// 配置HttpApi
        /// </summary>
        /// <param name="services"></param>
        /// <param name="httpApiType"></param>
        /// <param name="configureOptions"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IServiceCollection ConfigureHttpApi(this IServiceCollection services, Type httpApiType, Action<HttpApiOptions> configureOptions)
        {
            var builder = services.CreateHttpApiBuilder(httpApiType);
            return builder.ConfigureHttpApi(configureOptions);
        }

        /// <summary>
        /// 配置HttpApi
        /// </summary>
        /// <param name="services"></param>
        /// <param name="httpApiType"></param>
        /// <param name="configureOptions"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IServiceCollection ConfigureHttpApi(this IServiceCollection services, Type httpApiType, IConfiguration configureOptions)
        {
            var builder = services.CreateHttpApiBuilder(httpApiType);
            return builder.ConfigureHttpApi(configureOptions);
        }

        /// <summary>
        /// 创建HttpApiBuilder
        /// </summary>
        /// <param name="services"></param>
        /// <param name="httpApiType"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        private static IHttpApiBuilder CreateHttpApiBuilder(this IServiceCollection services, Type httpApiType)
        {
            if (httpApiType == null)
            {
                throw new ArgumentNullException(nameof(httpApiType));
            }

            var builderType = typeof(HttpApiBuilder<>).MakeGenericType(httpApiType);
            return Lambda.CreateCtorFunc<IServiceCollection, IHttpApiBuilder>(builderType)(services);
        }

        /// <summary>
        /// 定义httpApi的Builder的行为
        /// </summary>
        private interface IHttpApiBuilder
        {
            /// <summary>
            /// 添加HttpApi代理类到服务
            /// </summary>
            /// <param name="configureOptions"></param>
            /// <returns></returns>
            IHttpClientBuilder AddHttpApi(Action<HttpApiOptions, IServiceProvider> configureOptions);

            /// <summary>
            /// 配置HttpApi
            /// </summary>
            /// <param name="configureOptions"></param>
            /// <returns></returns>
            IServiceCollection ConfigureHttpApi(Action<HttpApiOptions> configureOptions);

            /// <summary>
            /// 配置HttpApi
            /// </summary>
            /// <param name="configureOptions"></param>
            /// <returns></returns>
            IServiceCollection ConfigureHttpApi(IConfiguration configureOptions);
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
            /// <param name="configureOptions"></param>
            /// <returns></returns>
            public IHttpClientBuilder AddHttpApi(Action<HttpApiOptions, IServiceProvider> configureOptions)
            {
                return this.services.AddHttpApi<THttpApi>((o, s) => configureOptions(o, s));
            }

            /// <summary>
            /// 配置HttpApi
            /// </summary> 
            /// <param name="configureOptions"></param>
            /// <returns></returns>
            public IServiceCollection ConfigureHttpApi(Action<HttpApiOptions> configureOptions)
            {
                return this.services.ConfigureHttpApi<THttpApi>(o => configureOptions(o));
            }

            /// <summary>
            /// 配置HttpApi
            /// </summary>
            /// <param name="configureOptions"></param>
            /// <returns></returns>
            public IServiceCollection ConfigureHttpApi(IConfiguration configureOptions)
            {
                return this.services.ConfigureHttpApi<THttpApi>(configureOptions);
            }
        }
    }
}

