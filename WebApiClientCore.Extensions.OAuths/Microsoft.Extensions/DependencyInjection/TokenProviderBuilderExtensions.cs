using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using WebApiClientCore.Extensions.OAuths.TokenProviders;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 提供TokenProviderBuilder的扩展
    /// </summary>
    public static class TokenProviderBuilderExtensions
    {
        /// <summary>
        /// 配置ClientCredentialsOptions
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions">配置</param>
        /// <returns></returns>
        public static ITokenProviderBuilder ConfigureClientCredentials(this ITokenProviderBuilder builder, Action<ClientCredentialsOptions, IServiceProvider> configureOptions)
        {
            builder.AddClientCredentialsOptions().Configure(configureOptions);
            return builder;
        }

        /// <summary>
        /// 配置ClientCredentialsOptions
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions">配置</param>
        /// <returns></returns>
        public static ITokenProviderBuilder ConfigureClientCredentials(this ITokenProviderBuilder builder, Action<ClientCredentialsOptions> configureOptions)
        {
            builder.AddClientCredentialsOptions().Configure(configureOptions);
            return builder;
        }

        /// <summary>
        /// 配置ClientCredentialsOptions
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configuration">配置</param>
        /// <returns></returns>
        public static ITokenProviderBuilder ConfigureClientCredentials(this ITokenProviderBuilder builder, IConfiguration configuration)
        {
            builder.AddClientCredentialsOptions().Bind(configuration);
            return builder;
        }


        /// <summary>
        /// 配置PasswordCredentialsOptions
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions">配置</param>
        /// <returns></returns>
        public static ITokenProviderBuilder ConfigurePasswordCredentials(this ITokenProviderBuilder builder, Action<PasswordCredentialsOptions, IServiceProvider> configureOptions)
        {
            builder.AddPasswordCredentialsOptions().Configure(configureOptions);
            return builder;
        }

        /// <summary>
        /// 配置PasswordCredentialsOptions
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions">配置</param>
        /// <returns></returns>
        public static ITokenProviderBuilder ConfigurePasswordCredentials(this ITokenProviderBuilder builder, Action<PasswordCredentialsOptions> configureOptions)
        {
            builder.AddPasswordCredentialsOptions().Configure(configureOptions);
            return builder;
        }

        /// <summary>
        /// 配置PasswordCredentialsOptions
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configuration">配置</param>
        /// <returns></returns>
        public static ITokenProviderBuilder ConfigurePasswordCredentials(this ITokenProviderBuilder builder, IConfiguration configuration)
        {
            builder.AddPasswordCredentialsOptions().Bind(configuration);
            return builder;
        }


        /// <summary>
        /// 配置ClientCredentialsOptions
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        private static OptionsBuilder<ClientCredentialsOptions> AddClientCredentialsOptions(this ITokenProviderBuilder builder)
        {
            return builder.AddOptions<ClientCredentialsOptions>();
        }

        /// <summary>
        /// 配置PasswordCredentialsOptions
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        private static OptionsBuilder<PasswordCredentialsOptions> AddPasswordCredentialsOptions(this ITokenProviderBuilder builder)
        {
            return builder.AddOptions<PasswordCredentialsOptions>();
        }

        /// <summary>
        /// 使用token提供者的名称创建指定类型的OptionsBuilder
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        private static OptionsBuilder<TOptions> AddOptions<TOptions>(this ITokenProviderBuilder builder) where TOptions : class
        {
            return builder.Services.AddOptions<TOptions>(builder.Name);
        }
    }
}
