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
        /// 配置HttpApi
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
        /// 配置HttpApi
        /// </summary> 
        /// <param name="services"></param>
        /// <param name="httpApiType">接口类型</param> 
        /// <param name="configureOptions">配置选项</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureHttpApi(this IServiceCollection services, Type httpApiType, Action<HttpApiOptions> configureOptions)
        {
            return services.Configure(httpApiType.FullName, configureOptions);
        }

        /// <summary>
        /// 配置HttpApi
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
        /// 配置HttpApi
        /// </summary> 
        /// <param name="services"></param>
        /// <param name="httpApiType">接口类型</param> 
        /// <param name="configureOptions">配置选项</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureHttpApi(this IServiceCollection services, Type httpApiType, IConfiguration configureOptions)
        {
            return services.Configure<HttpApiOptions>(httpApiType.FullName, configureOptions);
        }
    }
}
