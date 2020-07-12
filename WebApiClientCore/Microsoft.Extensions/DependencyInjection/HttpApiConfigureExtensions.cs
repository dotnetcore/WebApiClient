using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using System;
using System.Linq;
using WebApiClientCore;
using WebApiClientCore.Serialization;

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

        /// <summary>
        /// 配置HttpApi序列化json时使用NewtonsoftJson
        /// </summary>
        /// <param name="services"></param>
        /// <param name="optionAction"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureHttpApiUseNewtonsoftJson(this IServiceCollection services, Action<JsonSerializerSettings?>? optionAction = null)
        {
            Type serviceType = typeof(IJsonSerializer);
            ServiceDescriptor sd = services.FirstOrDefault(t => t.ServiceType == serviceType);

            if (sd == null)//未添加过，需要添加
            {
                services.AddSingleton<IJsonSerializer, NewtonsoftJsonSerializer>(t => NewtonsoftJsonSerializer.CreateJsonSerializer(optionAction));
                return services;
            }

            Type implementationType = typeof(NewtonsoftJsonSerializer);
            if (sd.ImplementationType != implementationType)//需要替换
            {
                services.Remove(sd);
                services.AddSingleton<IJsonSerializer, NewtonsoftJsonSerializer>(t => NewtonsoftJsonSerializer.CreateJsonSerializer(optionAction));
                return services;
            }

            //不需要替换
            return services;
        }
    }
}
