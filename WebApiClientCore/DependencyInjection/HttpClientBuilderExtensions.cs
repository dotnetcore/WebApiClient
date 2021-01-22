using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using WebApiClientCore;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// HttpClientBuilder扩展
    /// </summary>
    public static class HttpClientBuilderExtensions
    {
        /// <summary>
        /// 配置HttpApiOptions
        /// </summary>
        /// <param name="builder"></param>  
        /// <param name="configureOptions">配置选项</param>
        /// <returns></returns>
        public static IHttpClientBuilder ConfigureHttpApi(this IHttpClientBuilder builder, Action<HttpApiOptions, IServiceProvider> configureOptions)
        {
            builder.AddHttpApiOptions().Configure(configureOptions);
            return builder;
        }

        /// <summary>
        /// 配置HttpApiOptions
        /// </summary>
        /// <param name="builder"></param>  
        /// <param name="configureOptions">配置选项</param>
        /// <returns></returns>
        public static IHttpClientBuilder ConfigureHttpApi(this IHttpClientBuilder builder, Action<HttpApiOptions> configureOptions)
        {
            builder.AddHttpApiOptions().Configure(configureOptions);
            return builder;
        }

        /// <summary>
        /// 配置HttpApiOptions
        /// </summary>
        /// <param name="builder"></param>  
        /// <param name="configuration">配置</param>
        /// <returns></returns>
        public static IHttpClientBuilder ConfigureHttpApi(this IHttpClientBuilder builder, IConfiguration configuration)
        {
            builder.AddHttpApiOptions().Bind(configuration);
            return builder;
        }

        /// <summary>
        /// 添加HttpApiOptions
        /// </summary>
        /// <param name="builder"></param>   
        /// <returns></returns>
        private static OptionsBuilder<HttpApiOptions> AddHttpApiOptions(this IHttpClientBuilder builder)
        {
            return builder.Services.AddOptions<HttpApiOptions>(builder.Name);
        }
    }
}
