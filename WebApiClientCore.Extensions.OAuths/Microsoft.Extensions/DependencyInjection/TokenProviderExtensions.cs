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
            return services.AddTokenProvider<THttpApi, DelegateTokenProvider>(s => new DelegateTokenProvider(s, tokenRequest));
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
            return services
                .RemoveAll<TTokenProvider>()
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
        public static ITokenProviderBuilder AddTokenProvider<THttpApi, TTokenProvider>(this IServiceCollection services)
            where TTokenProvider : class, ITokenProvider
        {
            return services
                .RemoveAll<TTokenProvider>()
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
            var name = $"{typeof(TTokenProvider).Name}+{typeof(THttpApi).Name}";
            var hashCode = HashCode.Combine(typeof(THttpApi), typeof(TTokenProvider));
            name = hashCode < 0 ? $"{name}{hashCode}" : $"{name}+{hashCode}";

            services
               .AddOptions<TokenProviderFactoryOptions>()
               .Configure(o => o.Register<THttpApi, TTokenProvider>(name));

            services.TryAddSingleton<ITokenProviderFactory, TokenProviderFactory>();
            services.AddHttpApi<IOAuthTokenClientApi>(o => o.KeyValueSerializeOptions.IgnoreNullValues = true);

            return new TokenProviderBuilder(name, services);
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