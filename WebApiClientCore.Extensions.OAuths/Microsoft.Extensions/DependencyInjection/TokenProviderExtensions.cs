using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Extensions.OAuths;
using WebApiClientCore.Extensions.OAuths.TokenClients;
using WebApiClientCore.Extensions.OAuths.TokenProviders;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// token提供者扩展
    /// </summary>
    public static class TokenProviderExtensions
    {
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
            var provider = ServiceDescriptor.Singleton<ITokenProvider<THttpApi>, ClientCredentialsTokenProvider<THttpApi>>();
            services.Replace(provider);

            services.AddHttpApi<IOAuthTokenClientApi>(o => o.KeyValueSerializeOptions.IgnoreNullValues = true);
            return services.AddOptions<ClientCredentialsOptions>(HttpApi.GetName<THttpApi>());
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
            var provider = ServiceDescriptor.Singleton<ITokenProvider<THttpApi>, PasswordCredentialsTokenProvider<THttpApi>>();
            services.Replace(provider);

            services.AddHttpApi<IOAuthTokenClientApi>(o => o.KeyValueSerializeOptions.IgnoreNullValues = true);
            return services.AddOptions<PasswordCredentialsOptions>(HttpApi.GetName<THttpApi>());
        }




        /// <summary>
        /// 添加接口的自定义token提供者
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <param name="tokenRequest"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomTokenProvider<THttpApi>(this IServiceCollection services, Func<IServiceProvider, Task<TokenResult?>> tokenRequest)
        {
            return services.AddCustomTokenProvider<THttpApi, DelegateCustomTokenClient<THttpApi>>(s => new DelegateCustomTokenClient<THttpApi>(s, tokenRequest));
        }

        /// <summary>
        /// 添加接口的自定义token提供者
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <typeparam name="TCustomTokenClient">自定义token请求客户端</typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomTokenProvider<THttpApi, TCustomTokenClient>(this IServiceCollection services)
        {
            var serviceType = typeof(ICustomTokenClient<THttpApi>);
            var implementationType = typeof(TCustomTokenClient);
            var client = ServiceDescriptor.Transient(serviceType, implementationType);
            services.Replace(client);

            return services.AddCustomTokenProviderCore<THttpApi, TCustomTokenClient>();
        }

        /// <summary>
        /// 添加接口的自定义token提供者
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <typeparam name="TCustomTokenClient">自定义token请求客户端</typeparam>
        /// <param name="services"></param>
        /// <param name="factory">token客户端工厂</param>
        /// <returns></returns>
        public static IServiceCollection AddCustomTokenProvider<THttpApi, TCustomTokenClient>(this IServiceCollection services, Func<IServiceProvider, TCustomTokenClient> factory)
        {
            var serviceType = typeof(ICustomTokenClient<THttpApi>);
            var client = ServiceDescriptor.Transient(serviceType, s => factory(s));
            services.Replace(client);

            return services.AddCustomTokenProviderCore<THttpApi, TCustomTokenClient>();
        }

        /// <summary>
        /// 添加接口的自定义token提供者
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <typeparam name="TCustomTokenClient">自定义token请求客户端</typeparam>
        /// <param name="services"></param> 
        /// <returns></returns>
        private static IServiceCollection AddCustomTokenProviderCore<THttpApi, TCustomTokenClient>(this IServiceCollection services)
        {
            // 检测实现类型
            var serviceType = typeof(ICustomTokenClient<THttpApi>);
            var implementationType = typeof(TCustomTokenClient);
            if (serviceType.IsAssignableFrom(implementationType) == false)
            {
                var type = typeof(CustomTokenClient<>).MakeGenericType(typeof(THttpApi));
                throw new ArgumentException($"{typeof(TCustomTokenClient).Name}的类型必须为{type}");
            }

            var provider = ServiceDescriptor.Singleton<ITokenProvider<THttpApi>, CustomTokenProvider<THttpApi>>();
            return services.Replace(provider);
        }
    }
}
