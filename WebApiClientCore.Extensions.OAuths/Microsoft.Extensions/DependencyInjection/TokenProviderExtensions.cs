using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
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
        /// 为指定接口添加Client模式的token提供者
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions">配置</param>
        /// <returns></returns>
        public static IServiceCollection AddClientCredentialsTokenProvider<THttpApi>(this IServiceCollection services, Action<ClientCredentialsOptions, IServiceProvider> configureOptions)
        {
            return services.AddClientCredentialsTokenProvider<THttpApi>().Configure(configureOptions).Services;
        }

        /// <summary>
        /// 为指定接口添加Client模式的token提供者
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions">配置</param>
        /// <returns></returns>
        public static IServiceCollection AddClientCredentialsTokenProvider<THttpApi>(this IServiceCollection services, Action<ClientCredentialsOptions> configureOptions)
        {
            return services.AddClientCredentialsTokenProvider<THttpApi>().Configure(configureOptions).Services;
        }

        /// <summary>
        /// 为指定接口添加Client模式的token提供者
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <param name="services"></param>
        /// <returns></returns>     
        public static OptionsBuilder<ClientCredentialsOptions> AddClientCredentialsTokenProvider<THttpApi>(this IServiceCollection services)
        {
            var builder = services.AddTokeProvider<THttpApi, ClientCredentialsTokenProvider>();
            return builder.AddOptions<ClientCredentialsOptions>();
        }




        /// <summary>
        /// 为指定接口添加Password模式的token提供者
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions">配置</param>
        /// <returns></returns>
        public static IServiceCollection AddPasswordCredentialsTokenProvider<THttpApi>(this IServiceCollection services, Action<PasswordCredentialsOptions, IServiceProvider> configureOptions)
        {
            return services.AddPasswordCredentialsTokenProvider<THttpApi>().Configure(configureOptions).Services;
        }

        /// <summary>
        /// 为指定接口添加Password模式的token提供者
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions">配置</param>
        /// <returns></returns>
        public static IServiceCollection AddPasswordCredentialsTokenProvider<THttpApi>(this IServiceCollection services, Action<PasswordCredentialsOptions> configureOptions)
        {
            return services.AddPasswordCredentialsTokenProvider<THttpApi>().Configure(configureOptions).Services;
        }

        /// <summary>
        /// 为指定接口添加Password模式的token提供者
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static OptionsBuilder<PasswordCredentialsOptions> AddPasswordCredentialsTokenProvider<THttpApi>(this IServiceCollection services)
        {
            var builder = services.AddTokeProvider<THttpApi, PasswordCredentialsTokenProvider>();
            return builder.AddOptions<PasswordCredentialsOptions>();
        }



        /// <summary>
        /// 为指定接口添加token提供者
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <param name="services"></param>
        /// <param name="tokenRequest">token请求委托</param>
        /// <returns></returns>
        public static ITokenProviderBuilder AddTokeProvider<THttpApi>(this IServiceCollection services, Func<IServiceProvider, Task<TokenResult?>> tokenRequest)
        {
            return services.AddTokeProvider<THttpApi, DelegateTokenProvider>(s =>
            {
                return new DelegateTokenProvider(s, tokenRequest);
            });
        }

        /// <summary>
        /// 为指定接口添加token提供者
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <typeparam name="TTokenProvider">token提供者类型</typeparam>
        /// <param name="services"></param>
        /// <param name="tokenProviderFactory">token提供者创建工厂</param>
        /// <returns></returns>
        public static ITokenProviderBuilder AddTokeProvider<THttpApi, TTokenProvider>(this IServiceCollection services, Func<IServiceProvider, TTokenProvider> tokenProviderFactory)
            where TTokenProvider : class, ITokenProvider
        {
            return services
                .AddTransient(tokenProviderFactory)
                .AddTokenProviderCore<THttpApi, TTokenProvider>();
        }

        /// <summary>
        /// 为指定接口添加token提供者
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <typeparam name="TTokenProvider">token提供者类型</typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static ITokenProviderBuilder AddTokeProvider<THttpApi, TTokenProvider>(this IServiceCollection services)
            where TTokenProvider : class, ITokenProvider
        {
            return services
                .AddTransient<TTokenProvider>()
                .AddTokenProviderCore<THttpApi, TTokenProvider>();
        }

        /// <summary>
        /// 向token工厂提供者添加token提供者
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <typeparam name="TTokenProvider">token提供者类型</typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        private static ITokenProviderBuilder AddTokenProviderCore<THttpApi, TTokenProvider>(this IServiceCollection services)
            where TTokenProvider : class, ITokenProvider
        {
            var tokenProviderName = typeof(THttpApi).FullName;

            services
               .AddOptions<TokenProviderOptions>()
               .Configure(o => o.Register<THttpApi, TTokenProvider>(tokenProviderName));

            services.TryAddSingleton<ITokenProviderFactory, TokenProviderFactory>();
            services.AddHttpApi<IOAuthTokenClientApi>(o => o.KeyValueSerializeOptions.IgnoreNullValues = true);
            return new DefaultTokenProviderBuilder(tokenProviderName, services);
        }
    }
}