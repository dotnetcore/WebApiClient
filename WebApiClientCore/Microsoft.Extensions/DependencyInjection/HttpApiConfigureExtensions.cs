using Microsoft.Extensions.Configuration;
using System;
using WebApiClientCore;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// HttpApi配置控制
    /// </summary>
    public static class HttpApiConfigureExtensions
    {
        /// <summary>
        /// 配置HttpApi的选项
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions">配置选项</param>
        /// <returns></returns>
        public static IServiceCollection PostConfigureHttpApi<THttpApi>(this IServiceCollection services, Action<HttpApiOptions, IServiceProvider> configureOptions)
        {
            return services.PostConfigureHttpApi(typeof(THttpApi), configureOptions);
        }

        /// <summary>
        /// 配置HttpApi的选项
        /// </summary> 
        /// <param name="services"></param>
        /// <param name="httpApiType">接口类型</param> 
        /// <param name="configureOptions">配置选项</param>
        /// <returns></returns>
        public static IServiceCollection PostConfigureHttpApi(this IServiceCollection services, Type httpApiType, Action<HttpApiOptions, IServiceProvider> configureOptions)
        {
            var name = httpApiType.FullName;
            return services.AddOptions<HttpApiOptions>(name).PostConfigure(configureOptions).Services;
        }



        /// <summary>
        /// 配置HttpApi的选项
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions">配置选项</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureHttpApi<THttpApi>(this IServiceCollection services, Action<HttpApiOptions, IServiceProvider> configureOptions)
        {
            return services.ConfigureHttpApi(typeof(THttpApi), configureOptions);
        }

        /// <summary>
        /// 配置HttpApi的选项
        /// </summary> 
        /// <param name="services"></param>
        /// <param name="httpApiType">接口类型</param> 
        /// <param name="configureOptions">配置选项</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureHttpApi(this IServiceCollection services, Type httpApiType, Action<HttpApiOptions, IServiceProvider> configureOptions)
        {
            var name = httpApiType.FullName;
            return services.AddOptions<HttpApiOptions>(name).Configure(configureOptions).Services;
        }



        /// <summary>
        /// 配置HttpApi的选项
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions">配置选项</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureHttpApi<THttpApi>(this IServiceCollection services, Action<HttpApiOptions> configureOptions)
        {
            return services.ConfigureHttpApi(typeof(THttpApi), configureOptions);
        }

        /// <summary>
        /// 配置HttpApi的选项
        /// </summary> 
        /// <param name="services"></param>
        /// <param name="httpApiType">接口类型</param> 
        /// <param name="configureOptions">配置选项</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureHttpApi(this IServiceCollection services, Type httpApiType, Action<HttpApiOptions> configureOptions)
        {
            var name = httpApiType.FullName;
            return services.AddOptions<HttpApiOptions>(name).Configure(configureOptions).Services;
        }



        /// <summary>
        /// 配置HttpApi的选项
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions">配置选项</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureHttpApi<THttpApi>(this IServiceCollection services, IConfiguration configureOptions)
        {
            return services.ConfigureHttpApi(typeof(THttpApi), configureOptions);
        }

        /// <summary>
        /// 配置HttpApi的选项
        /// </summary> 
        /// <param name="services"></param>
        /// <param name="httpApiType">接口类型</param> 
        /// <param name="configureOptions">配置选项</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureHttpApi(this IServiceCollection services, Type httpApiType, IConfiguration configureOptions)
        {
            var name = httpApiType.FullName;
            return services.AddOptions<HttpApiOptions>(name).Bind(configureOptions).Services;
        }




        /// <summary>
        /// 配置HttpApi的选项
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions">配置选项</param>
        public static IHttpClientBuilder ConfigureHttpApi(this IHttpClientBuilder builder, Action<HttpApiOptions, IServiceProvider> configureOptions)
        {
            builder.Services.AddOptions<HttpApiOptions>(builder.Name).Configure(configureOptions);
            return builder;
        }

        /// <summary>
        /// 配置HttpApi的选项
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions">配置选项</param>
        public static IHttpClientBuilder ConfigureHttpApi(this IHttpClientBuilder builder, Action<HttpApiOptions> configureOptions)
        {
            builder.Services.AddOptions<HttpApiOptions>(builder.Name).Configure(configureOptions);
            return builder;
        }

        /// <summary>
        /// 配置HttpApi的选项
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions">配置选项</param>
        /// <returns></returns>
        public static IHttpClientBuilder ConfigureHttpApi(this IHttpClientBuilder builder, IConfiguration configureOptions)
        {
            builder.Services.AddOptions<HttpApiOptions>(builder.Name).Bind(configureOptions);
            return builder;
        }
    }
}
