using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using WebApiClientCore.Extensions.OAuths;
using WebApiClientCore.Extensions.OAuths.TokenProviders;

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
        /// <returns></returns>
        public static IServiceCollection AddClientCredentialsTokenProvider<THttpApi>(this IServiceCollection services)
        {
            services.TryAddSingleton<IClientCredentialsTokenProvider<THttpApi>, ClientCredentialsTokenProvider<THttpApi>>();
            return services.AddTokenProvider();
        }

        /// <summary>
        /// 添加接口的Client身份读取token提供者
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddClientCredentialsTokenProvider<THttpApi>(this IServiceCollection services, Action<ClientCredentialsOptions> configureOptions)
        {
            services.AddOptions<ClientCredentialsOptions>(typeof(THttpApi).FullName).Configure(configureOptions);
            return services.AddClientCredentialsTokenProvider<THttpApi>();
        }

        /// <summary>
        /// 添加接口的Client身份读取token提供者
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddClientCredentialsTokenProvider<THttpApi>(this IServiceCollection services, Action<ClientCredentialsOptions, IServiceProvider> configureOptions)
        {
            services.AddOptions<ClientCredentialsOptions>(typeof(THttpApi).FullName).Configure(configureOptions);
            return services.AddClientCredentialsTokenProvider<THttpApi>();
        }

        /// <summary>
        /// 添加接口的Password身份读取token提供者
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPasswordCredentialsTokenProvider<THttpApi>(this IServiceCollection services)
        {
            services.TryAddSingleton<IClientCredentialsTokenProvider<THttpApi>, ClientCredentialsTokenProvider<THttpApi>>();
            return services.AddTokenProvider();
        }

        /// <summary>
        /// 添加接口的Password身份读取token提供者
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddPasswordCredentialsTokenProvider<THttpApi>(this IServiceCollection services, Action<PasswordCredentialsOptions> configureOptions)
        {
            services.AddOptions<PasswordCredentialsOptions>(typeof(THttpApi).FullName).Configure(configureOptions);
            return services.AddPasswordCredentialsTokenProvider<THttpApi>();
        }

        /// <summary>
        /// 添加接口的Password身份读取token提供者
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddPasswordCredentialsTokenProvider<THttpApi>(this IServiceCollection services, Action<PasswordCredentialsOptions, IServiceProvider> configureOptions)
        {
            services.AddOptions<PasswordCredentialsOptions>(typeof(THttpApi).FullName).Configure(configureOptions);
            return services.AddPasswordCredentialsTokenProvider<THttpApi>();
        }

        /// <summary>
        /// 添加TokenProvider依赖项
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddTokenProvider(this IServiceCollection services)
        {
            services.AddOptions();
            services.AddHttpApi<IOAuthClient>(o => o.KeyValueSerializeOptions.IgnoreNullValues = true);
            return services;
        }
    }
}
