using System;
using WebApiClientCore.Extensions.NewtonsoftJson;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 提供HttpApi相关扩展
    /// </summary>
    public static class HttpClientBuilderExtensions
    {
        /// <summary>
        /// 为接口配置JsonNetSerializerOptions
        /// </summary> 
        /// <param name="builder"></param>
        /// <param name="configureOptions">配置选项</param>
        /// <returns></returns>
        public static IHttpClientBuilder ConfigureNewtonsoftJson(this IHttpClientBuilder builder, Action<JsonNetSerializerOptions> configureOptions)
        {
            builder.Services.AddOptions<JsonNetSerializerOptions>(builder.Name).Configure(configureOptions);
            return builder;
        }

        /// <summary>
        /// 为接口配置JsonNetSerializerOptions
        /// </summary> 
        /// <param name="builder"></param>
        /// <param name="configureOptions">配置选项</param>
        /// <returns></returns>
        public static IHttpClientBuilder ConfigureNewtonsoftJson(this IHttpClientBuilder builder, Action<JsonNetSerializerOptions, IServiceProvider> configureOptions)
        {
            builder.Services.AddOptions<JsonNetSerializerOptions>(builder.Name).Configure(configureOptions);
            return builder;
        }
    }
}
