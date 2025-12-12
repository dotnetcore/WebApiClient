using Microsoft.Extensions.DependencyInjection;
using System;

namespace WebApiClientCore.Extensions.OAuths.DependencyInjection
{
    /// <summary>
    /// OAuthToken配置的依赖注入扩展
    /// </summary>
    public static class OAuthTokenOptionsExtensions
    {
        /// <summary>
        /// 配置OAuth Token刷新选项
        /// 使用独立的配置选项，支持从appsettings.json加载
        /// </summary>
        /// <param name="builder">HttpClient构建器</param>
        /// <param name="configure">配置委托</param>
        /// <returns>HttpClient构建器</returns>
        public static IHttpClientBuilder ConfigureOAuthTokenOptions(
            this IHttpClientBuilder builder, 
            Action<OAuthTokenOptions> configure)
        {
            builder.Services.Configure<OAuthTokenOptions>(builder.Name, configure);
            return builder;
        }

        /// <summary>
        /// 配置OAuth Token刷新选项（通过IConfiguration）
        /// 支持从appsettings.json等配置源加载
        /// </summary>
        /// <param name="builder">HttpClient构建器</param>
        /// <param name="configuration">配置节</param>
        /// <returns>HttpClient构建器</returns>
        public static IHttpClientBuilder ConfigureOAuthTokenOptions(
            this IHttpClientBuilder builder,
            Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            builder.Services.Configure<OAuthTokenOptions>(builder.Name, configuration);
            return builder;
        }
    }
}
