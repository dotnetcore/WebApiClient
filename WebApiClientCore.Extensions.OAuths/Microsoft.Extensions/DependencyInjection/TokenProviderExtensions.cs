using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Threading.Tasks;
using WebApiClientCore.Extensions.OAuths;
using WebApiClientCore.Extensions.OAuths.TokenProviders;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// token提供者扩展
    /// </summary>
    public static partial class TokenProviderExtensions
    {
        /// <summary>
        /// 为指定接口添加token提供者
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <param name="services"></param>
        /// <param name="tokenRequest">token请求委托</param>
        /// <returns></returns>
        public static ITokenProviderBuilder AddTokenProvider<THttpApi>(this IServiceCollection services, Func<IServiceProvider, Task<TokenResult?>> tokenRequest)
        {
            return services.AddTokenProvider<THttpApi>(string.Empty, tokenRequest);
        }

        /// <summary>
        /// 为指定接口添加token提供者
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <param name="services"></param>
        /// <param name="tokenProviderName">token提供者的名称</param>
        /// <param name="tokenRequest">token请求委托</param>
        /// <returns></returns>
        public static ITokenProviderBuilder AddTokenProvider<THttpApi>(this IServiceCollection services, string tokenProviderName, Func<IServiceProvider, Task<TokenResult?>> tokenRequest)
        {
            return services.AddTokenProvider<THttpApi, DelegateTokenProvider>(tokenProviderName, s =>
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
        public static ITokenProviderBuilder AddTokenProvider<THttpApi, TTokenProvider>(this IServiceCollection services, Func<IServiceProvider, TTokenProvider> tokenProviderFactory)
            where TTokenProvider : class, ITokenProvider
        {
            return services.AddTokenProvider<THttpApi, TTokenProvider>(string.Empty, tokenProviderFactory);
        }


        /// <summary>
        /// 为指定接口添加token提供者
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <typeparam name="TTokenProvider">token提供者类型</typeparam>
        /// <param name="services"></param>
        /// <param name="tokenProviderName">token提供者的名称</param>
        /// <param name="tokenProviderFactory">token提供者创建工厂</param>
        /// <returns></returns>
        public static ITokenProviderBuilder AddTokenProvider<THttpApi, TTokenProvider>(this IServiceCollection services, string tokenProviderName, Func<IServiceProvider, TTokenProvider> tokenProviderFactory)
            where TTokenProvider : class, ITokenProvider
        {
            return services
                .AddTransient(tokenProviderFactory)
                .AddTokenProviderCore<THttpApi, TTokenProvider>(tokenProviderName);
        }


        /// <summary>
        /// 为指定接口添加token提供者
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <typeparam name="TTokenProvider">token提供者类型</typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static ITokenProviderBuilder AddTokenProvider<THttpApi, TTokenProvider>(this IServiceCollection services)
            where TTokenProvider : class, ITokenProvider
        {
            return services.AddTokenProvider<THttpApi, TTokenProvider>(string.Empty);
        }

        /// <summary>
        /// 为指定接口添加token提供者
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <typeparam name="TTokenProvider">token提供者类型</typeparam>
        /// <param name="services"></param>
        /// <param name="tokenProviderName">token提供者的名称</param>
        /// <returns></returns>
        public static ITokenProviderBuilder AddTokenProvider<THttpApi, TTokenProvider>(this IServiceCollection services, string tokenProviderName)
            where TTokenProvider : class, ITokenProvider
        {
            return services
                .AddTransient<TTokenProvider>()
                .AddTokenProviderCore<THttpApi, TTokenProvider>(tokenProviderName);
        }


        /// <summary>
        /// 向token工厂提供者添加token提供者
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <typeparam name="TTokenProvider">token提供者类型</typeparam>
        /// <param name="services"></param>
        /// <param name="tokenProviderName">名称</param>
        /// <returns></returns>
        private static ITokenProviderBuilder AddTokenProviderCore<THttpApi, TTokenProvider>(this IServiceCollection services, string? tokenProviderName)
            where TTokenProvider : class, ITokenProvider
        {
            if (string.IsNullOrEmpty(tokenProviderName) == true)
            {
                tokenProviderName = typeof(THttpApi).FullName;
            }

            services
               .AddOptions<TokenProviderOptions>()
               .Configure(o => o.Register<THttpApi, TTokenProvider>(tokenProviderName));

            services.TryAddSingleton<ITokenProviderFactory, TokenProviderFactory>();
            services.AddHttpApi<IOAuthTokenClientApi>(o => o.KeyValueSerializeOptions.IgnoreNullValues = true);
            return new TokenProviderBuilder(tokenProviderName, services);
        }


        /// <summary>
        /// token提供者创建器
        /// </summary>
        private class TokenProviderBuilder : ITokenProviderBuilder
        {
            /// <summary>
            /// 获取token提供者的别名
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// 获取服务描述集合
            /// </summary>
            public IServiceCollection Services { get; }

            /// <summary>
            /// token提供者创建器
            /// </summary>
            /// <param name="name">token提供者的别名</param>
            /// <param name="services">服务描述集合</param>
            public TokenProviderBuilder(string name, IServiceCollection services)
            {
                this.Name = name;
                this.Services = services;
            }
        }
    }
}