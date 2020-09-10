using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel;
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
        /// 添加Client身份读取token提供者
        /// </summary> 
        /// <param name="services"></param>
        /// <param name="setup">配置</param>
        /// <returns></returns>
        public static IServiceCollection AddClientCredentialsTokenProvider(this IServiceCollection services, Action<IClientCredentialsOptionsBuilder> setup)
        {
            setup?.Invoke(new ClientCredentialsOptionsBuilder(services));
            return services.AddClientCredentialsTokenProvider();
        }

        /// <summary>
        /// 添加和配置Client身份读取token提供者
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddClientCredentialsTokenProvider<THttpApi>(this IServiceCollection services, Action<ClientCredentialsOptions, IServiceProvider> configureOptions)
        {
            return services.AddClientCredentialsTokenProvider<THttpApi>().Configure(configureOptions).Services;
        }

        /// <summary>
        /// 添加和配置Client身份读取token提供者
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddClientCredentialsTokenProvider<THttpApi>(this IServiceCollection services, Action<ClientCredentialsOptions> configureOptions)
        {
            return services.AddClientCredentialsTokenProvider<THttpApi>().Configure(configureOptions).Services;
        }

        /// <summary>
        /// 添加和配置Client身份读取token提供者
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>     
        public static OptionsBuilder<ClientCredentialsOptions> AddClientCredentialsTokenProvider<THttpApi>(this IServiceCollection services)
        {
            services.AddClientCredentialsTokenProvider();
            return new ClientCredentialsOptionsBuilder(services).AddOptions<THttpApi>();
        }

        /// <summary>
        /// 添加Client身份读取token提供者
        /// </summary> 
        /// <param name="services"></param> 
        /// <returns></returns>
        private static IServiceCollection AddClientCredentialsTokenProvider(this IServiceCollection services)
        {
            services.AddTokenProvider();
            services.TryAddSingleton(typeof(IClientCredentialsTokenProvider<>), typeof(ClientCredentialsTokenProvider<>));
            return services;
        }





        /// <summary>
        /// 添加Password身份读取token提供者
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setup">配置</param>
        /// <returns></returns>
        public static IServiceCollection AddPasswordCredentialsTokenProvider(this IServiceCollection services, Action<IPasswordCredentialsOptionsBuilder> setup)
        {
            setup?.Invoke(new PasswordCredentialsOptionsBuilder(services));
            return services.AddPasswordCredentialsTokenProvider();
        }

        /// <summary>
        /// 添加和配置Password身份读取token提供者
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddPasswordCredentialsTokenProvider<THttpApi>(this IServiceCollection services, Action<PasswordCredentialsOptions, IServiceProvider> configureOptions)
        {
            return services.AddPasswordCredentialsTokenProvider<THttpApi>().Configure(configureOptions).Services;
        }

        /// <summary>
        /// 添加和配置Password身份读取token提供者
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddPasswordCredentialsTokenProvider<THttpApi>(this IServiceCollection services, Action<PasswordCredentialsOptions> configureOptions)
        {
            return services.AddPasswordCredentialsTokenProvider<THttpApi>().Configure(configureOptions).Services;
        }

        /// <summary>
        /// 添加和配置Password身份读取token提供者
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static OptionsBuilder<PasswordCredentialsOptions> AddPasswordCredentialsTokenProvider<THttpApi>(this IServiceCollection services)
        {
            services.AddPasswordCredentialsTokenProvider();
            return new PasswordCredentialsOptionsBuilder(services).AddOptions<THttpApi>();
        }

        /// <summary>
        /// 添加Password身份读取token提供者
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private static IServiceCollection AddPasswordCredentialsTokenProvider(this IServiceCollection services)
        {
            services.AddTokenProvider();
            services.TryAddSingleton(typeof(IPasswordCredentialsTokenProvider<>), typeof(PasswordCredentialsTokenProvider<>));
            return services;
        }



        /// <summary>
        /// 添加TokenProvider依赖项
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IServiceCollection AddTokenProvider(this IServiceCollection services)
        {
            services.AddHttpApi<IOAuthClient>();
            services.AddHttpApiOptions<IOAuthClient>().PostConfigure(o =>
            {
                o.KeyValueSerializeOptions.IgnoreNullValues = true;
            });
            return services;
        }
    }
}
