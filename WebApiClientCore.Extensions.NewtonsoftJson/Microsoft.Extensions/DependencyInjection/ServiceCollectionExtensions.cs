using Microsoft.Extensions.Options;
using System;
using WebApiClientCore;
using WebApiClientCore.Extensions.NewtonsoftJson;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 提供JsonNetSerializerOptions的配置扩展
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
            return services.AddNewtonsoftJsonOptions(typeof(THttpApi));
        }

        /// <summary>
        /// 为接口配置JsonNetSerializerOptions
        /// </summary>
        /// <param name="services"></param>
        /// <param name="httpApiType">接口类型</param>
        /// <returns></returns>
        public static OptionsBuilder<JsonNetSerializerOptions> AddNewtonsoftJsonOptions(this IServiceCollection services, Type httpApiType)
        {
            var name = HttpApi.GetName(httpApiType);
            return services.AddOptions<JsonNetSerializerOptions>(name);
        }
    }
}
