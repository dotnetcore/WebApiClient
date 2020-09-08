using Microsoft.Extensions.Options;
using System;
using WebApiClientCore.Extensions.NewtonsoftJson;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// HttpApi配置控制
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 为接口配置JsonNetSerializerOptions
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static OptionsBuilder<JsonNetSerializerOptions> AddNewtonsoftJsonOptions<THttpApi>(this IServiceCollection services)
        {
            var name = services.AddHttpApiOptions<THttpApi>().Name;
            return services.AddOptions<JsonNetSerializerOptions>(name);
        } 

        /// <summary>
        /// 为接口配置JsonNetSerializerOptions
        /// </summary>
        /// <param name="services"></param>
        /// <param name="httpApiType">接口类型</param>
        /// <returns></returns>
        public static OptionsBuilder<JsonNetSerializerOptions> AddNewtonsoftJsonOptions(this IServiceCollection services, Type httpApiType)
        {
            var name = services.AddHttpApiOptions(httpApiType).Name;
            return services.AddOptions<JsonNetSerializerOptions>(name);
        }
    }
}
