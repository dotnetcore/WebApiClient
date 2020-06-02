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
            return services.AddClientCredentialsTokenProvider<THttpApi>(o => { });
        }

        /// <summary>
        /// 添加接口的Client身份读取token提供者
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddClientCredentialsTokenProvider<THttpApi>(this IServiceCollection services, Action<ClientCredentialsOptions<THttpApi>> configureOptions)
        {
            return services.AddClientCredentialsTokenProvider<THttpApi>((o, s) => configureOptions(o));
        }

        /// <summary>
        /// 添加接口的Client身份读取token提供者
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddClientCredentialsTokenProvider<THttpApi>(this IServiceCollection services, Action<ClientCredentialsOptions<THttpApi>, IServiceProvider> configureOptions)
        {
            services.AddHttpApi<IOAuthClient>(o => o.KeyValueSerializeOptions.IgnoreNullValues = true);
            services.AddOptions<ClientCredentialsOptions<THttpApi>>().Configure(configureOptions);
            services.TryAddSingleton<IClientCredentialsTokenProvider<THttpApi>, ClientCredentialsTokenProvider<THttpApi>>();
            services.AddSingleton(typeof(ITokenProvider), typeof(ClientCredentialsTokenProvider<>).MakeGenericType(typeof(THttpApi)));
            return services;
        }


        /// <summary>
        /// 添加接口的Password身份读取token提供者
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPasswordCredentialsTokenProvider<THttpApi>(this IServiceCollection services)
        {
            return services.AddPasswordCredentialsTokenProvider<THttpApi>(o => { });
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
            return services.AddPasswordCredentialsTokenProvider<THttpApi>((o, s) => configureOptions(o));
        }

        /// <summary>
        /// 添加接口的Password身份读取token提供者
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddPasswordCredentialsTokenProvider<THttpApi>(this IServiceCollection services, Action<PasswordCredentialsOptions<THttpApi>, IServiceProvider> configureOptions)
        {
            services.AddHttpApi<IOAuthClient>(o => o.KeyValueSerializeOptions.IgnoreNullValues = true);
            services.AddOptions<PasswordCredentialsOptions<THttpApi>>().Configure(configureOptions);
            services.TryAddSingleton<IPasswordCredentialsTokenProvider<THttpApi>, PasswordCredentialsTokenProvider<THttpApi>>();
            services.AddSingleton(typeof(ITokenProvider), typeof(PasswordCredentialsTokenProvider<>).MakeGenericType(typeof(THttpApi)));
            return services;
        }
    }
}
