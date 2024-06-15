using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using WebApiClientCore;
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
        /// 为指定接口添加 token 提供者
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <param name="services"></param>
        /// <param name="tokenRequest">token请求委托</param>
        /// <param name="alias">TokenProvider的别名</param>
        /// <returns></returns>
        public static ITokenProviderBuilder AddTokenProvider<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] THttpApi>(
            this IServiceCollection services,
            Func<IServiceProvider, Task<TokenResult?>> tokenRequest,
            string alias = "")
        {
            return services.AddTokenProvider<THttpApi, DelegateTokenProvider>(s => new DelegateTokenProvider(s, tokenRequest), alias);
        }

        /// <summary>
        /// 为指定接口添加 token 提供者
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <typeparam name="TTokenProvider">token提供者类型</typeparam>
        /// <param name="services"></param>
        /// <param name="tokenProviderFactory">token提供者创建工厂</param>
        /// <param name="alias">TokenProvider的别名</param>
        /// <returns></returns>
        public static ITokenProviderBuilder AddTokenProvider<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] THttpApi,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TTokenProvider>(
            this IServiceCollection services,
            Func<IServiceProvider, TTokenProvider> tokenProviderFactory,
            string alias = "") where TTokenProvider : class, ITokenProvider
        {
            return services
                .RemoveAll<TTokenProvider>()
                .AddTransient(tokenProviderFactory)
                .AddTokenProviderCore<THttpApi, TTokenProvider>(alias);
        }


        /// <summary>
        /// 为指定接口添加 token 提供者
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <typeparam name="TTokenProvider">token提供者类型</typeparam>
        /// <param name="services"></param>
        /// <param name="alias">TokenProvider的别名</param>
        /// <returns></returns>
        public static ITokenProviderBuilder AddTokenProvider<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] THttpApi,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TTokenProvider>(
            this IServiceCollection services,
            string alias = "") where TTokenProvider : class, ITokenProvider
        {
            return services
                .RemoveAll<TTokenProvider>()
                .AddTransient<TTokenProvider>()
                .AddTokenProviderCore<THttpApi, TTokenProvider>(alias);
        }


        /// <summary>
        /// 向 token 工厂提供者添加 token 提供者
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <typeparam name="TTokenProvider">token提供者类型</typeparam>
        /// <param name="services"></param>
        /// <param name="alias">TokenProvider的别名</param>
        /// <returns></returns>
        private static TokenProviderBuilder AddTokenProviderCore<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] THttpApi,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TTokenProvider>(
            this IServiceCollection services,
            string alias) where TTokenProvider : class, ITokenProvider
        {
            if (alias == null)
            {
                throw new ArgumentNullException(nameof(alias));
            }

            services
               .AddOptions<TokenProviderFactoryOptions>()
               .Configure(o => o.Register<THttpApi, TTokenProvider>(alias));

            services
                .AddOptions<HttpApiOptions>(HttpApi.GetName(typeof(OAuth2TokenClient)))
                .Configure(o => o.KeyValueSerializeOptions.IgnoreNullValues = true);

            services.TryAddSingleton<OAuth2TokenClient>();
            services.TryAddTransient(typeof(TokenProviderService<,>));
            services.TryAddSingleton<ITokenProviderFactory, TokenProviderFactory>();

            var providerName = TokenProviderService<THttpApi, TTokenProvider>.GetTokenProviderName(alias);
            return new TokenProviderBuilder(providerName, services);
        }


        /// <summary>
        /// token提供者创建器
        /// </summary>
        private class TokenProviderBuilder : ITokenProviderBuilder
        {
            /// <summary>
            /// 获取 token 提供者的名称
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// 获取服务描述集合
            /// </summary>
            public IServiceCollection Services { get; }

            /// <summary>
            /// token提供者创建器
            /// </summary>
            /// <param name="name">token提供者的名称</param>
            /// <param name="services">服务描述集合</param>
            public TokenProviderBuilder(string name, IServiceCollection services)
            {
                this.Name = name;
                this.Services = services;
            }

            public override string ToString()
            {
                return this.Name;
            }
        }
    }
}