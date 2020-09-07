using Microsoft.Extensions.Configuration;
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
        /// 配置HttpApi的选项
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions">配置选项</param>
        public static IHttpClientBuilder ConfigureHttpApi(this IHttpClientBuilder builder, Action<HttpApiOptions> configureOptions)
        {
            builder.Services.Configure(builder.Name, configureOptions);
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
            builder.Services.Configure<HttpApiOptions>(builder.Name, configureOptions);
            return builder;
        }
    }
}
