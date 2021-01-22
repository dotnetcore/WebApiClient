using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using WebApiClientCore;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 提供HttpApiOptions配置扩展
    /// </summary>
    public static class HttpApiConfigureExtensions
    {
        /// <summary>
        /// 为接口配置HttpApiOptions
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions">配置选项</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureHttpApi<THttpApi>(this IServiceCollection services, Action<HttpApiOptions, IServiceProvider> configureOptions)
        {
            return services.AddHttpApiOptions<THttpApi>().Configure(configureOptions).Services;
        }

        /// <summary>
        /// 为接口配置HttpApiOptions
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions">配置选项</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureHttpApi<THttpApi>(this IServiceCollection services, Action<HttpApiOptions> configureOptions)
        {
            return services.AddHttpApiOptions<THttpApi>().Configure(configureOptions).Services;
        }

        /// <summary>
        /// 为接口配置HttpApiOptions
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions">配置选项</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureHttpApi<THttpApi>(this IServiceCollection services, IConfiguration configureOptions)
        {
            return services.AddHttpApiOptions<THttpApi>().Bind(configureOptions).Services;
        }



        /// <summary>
        /// 为接口配置HttpApiOptions
        /// </summary> 
        /// <param name="services"></param>
        /// <param name="httpApiType">接口类型</param> 
        /// <param name="configureOptions">配置选项</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureHttpApi(this IServiceCollection services, Type httpApiType, Action<HttpApiOptions, IServiceProvider> configureOptions)
        {
            return services.AddHttpApiOptions(httpApiType).Configure(configureOptions).Services;
        }

        /// <summary>
        /// 为接口配置HttpApiOptions
        /// </summary> 
        /// <param name="services"></param>
        /// <param name="httpApiType">接口类型</param> 
        /// <param name="configureOptions">配置选项</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureHttpApi(this IServiceCollection services, Type httpApiType, Action<HttpApiOptions> configureOptions)
        {
            return services.AddHttpApiOptions(httpApiType).Configure(configureOptions).Services;
        }

        /// <summary>
        /// 为接口配置HttpApiOptions
        /// </summary> 
        /// <param name="services"></param>
        /// <param name="httpApiType">接口类型</param> 
        /// <param name="configureOptions">配置选项</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureHttpApi(this IServiceCollection services, Type httpApiType, IConfiguration configureOptions)
        {
            return services.AddHttpApiOptions(httpApiType).Bind(configureOptions).Services;
        }

        /// <summary>
        /// 为接口配置HttpApiOptions
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        private static OptionsBuilder<HttpApiOptions> AddHttpApiOptions<THttpApi>(this IServiceCollection services)
        {
            return services.AddHttpApiOptions(typeof(THttpApi));
        }

        /// <summary>
        /// 为接口配置HttpApiOptions
        /// </summary> 
        /// <param name="services"></param>
        /// <param name="httpApiType">接口类型</param>  
        /// <returns></returns>
        private static OptionsBuilder<HttpApiOptions> AddHttpApiOptions(this IServiceCollection services, Type httpApiType)
        {
            var name = HttpApi.GetName(httpApiType);
            return services.AddOptions<HttpApiOptions>(name);
        }
    }
}
