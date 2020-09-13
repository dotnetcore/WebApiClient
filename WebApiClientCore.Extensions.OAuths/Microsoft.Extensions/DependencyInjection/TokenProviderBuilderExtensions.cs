using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using WebApiClientCore.Extensions.OAuths;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 提供TokenProviderBuilder的扩展
    /// </summary>
    public static class TokenProviderBuilderExtensions
    {
        /// <summary>
        /// 配置Client模式的授权信息
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configuration">配置</param>
        /// <returns></returns>
        public static ITokenProviderBuilder ConfigureClientCredentials(this ITokenProviderBuilder builder, IConfiguration configuration)
        {
            builder.AddOptions<ClientCredentialsOptions>().Bind(configuration);
            return builder;
        }

        /// <summary>
        /// 配置Client模式的授权信息
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions">配置</param>
        /// <returns></returns>
        public static ITokenProviderBuilder ConfigureClientCredentials(this ITokenProviderBuilder builder, Action<ClientCredentialsOptions> configureOptions)
        {
            builder.AddOptions<ClientCredentialsOptions>().Configure(configureOptions);
            return builder;
        }

        /// <summary>
        /// 配置Client模式的授权信息
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions">配置</param>
        /// <returns></returns>
        public static ITokenProviderBuilder ConfigureClientCredentials(this ITokenProviderBuilder builder, Action<ClientCredentialsOptions, IServiceProvider> configureOptions)
        {
            builder.AddOptions<ClientCredentialsOptions>().Configure(configureOptions);
            return builder;
        }




        /// <summary>
        /// 配置Password模式的授权信息
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configuration">配置</param>
        /// <returns></returns>
        public static ITokenProviderBuilder ConfigurePasswordCredentials(this ITokenProviderBuilder builder, IConfiguration configuration)
        {
            builder.AddOptions<PasswordCredentialsOptions>().Bind(configuration);
            return builder;
        }

        /// <summary>
        /// 配置Password模式的授权信息
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions">配置</param>
        /// <returns></returns>
        public static ITokenProviderBuilder ConfigurePasswordCredentials(this ITokenProviderBuilder builder, Action<PasswordCredentialsOptions> configureOptions)
        {
            builder.AddOptions<PasswordCredentialsOptions>().Configure(configureOptions);
            return builder;
        }

        /// <summary>
        /// 配置Password模式的授权信息
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions">配置</param>
        /// <returns></returns>
        public static ITokenProviderBuilder ConfigurePasswordCredentials(this ITokenProviderBuilder builder, Action<PasswordCredentialsOptions, IServiceProvider> configureOptions)
        {
            builder.AddOptions<PasswordCredentialsOptions>().Configure(configureOptions);
            return builder;
        }



        /// <summary>
        /// 使用token提供者的名称创建指定类型的OptionsBuilder
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static OptionsBuilder<TOptions> AddOptions<TOptions>(this ITokenProviderBuilder builder) where TOptions : class
        {
            return builder.Services.AddOptions<TOptions>(builder.Name);
        }
    }
}
