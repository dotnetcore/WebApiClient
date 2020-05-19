using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using WebApiClientCore.OAuths;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// token提供者扩展
    /// </summary>
    public static class TokenProviderExtensions
    {
        /// <summary>
        /// 添加接口的Client身份读取token提供者
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddClientCredentialsTokenProvider<THttpApi>(this IServiceCollection services, Action<ClientCredentialsOptions<THttpApi>> configureOptions)
        {
            return services.AddClientCredentialsTokenProvider<THttpApi>().Configure(configureOptions);
        }

        /// <summary>
        /// 添加接口的Client身份读取token提供者
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddClientCredentialsTokenProvider<THttpApi>(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddOptions<ClientCredentialsOptions<THttpApi>>();
            services.TryAddSingleton<ClientCredentialsTokenProvider<THttpApi>>();
            return services;
        }

        /// <summary>
        /// 添加接口的Password身份读取token提供者
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddPasswordCredentialsTokenProvider<THttpApi>(this IServiceCollection services, Action<PasswordCredentialsOptions<THttpApi>> configureOptions)
        {
            return services.AddPasswordCredentialsTokenProvider<THttpApi>().Configure(configureOptions);
        }

        /// <summary>
        /// 添加接口的Password身份读取token提供者
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPasswordCredentialsTokenProvider<THttpApi>(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddOptions<PasswordCredentialsOptions<THttpApi>>();
            services.TryAddSingleton<PasswordCredentialsOptions<THttpApi>>();
            return services;
        }
    }
}
