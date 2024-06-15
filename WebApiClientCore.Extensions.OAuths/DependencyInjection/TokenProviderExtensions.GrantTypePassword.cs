using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using WebApiClientCore.Extensions.OAuths.TokenProviders;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// token提供者扩展
    /// </summary>
    public static partial class TokenProviderExtensions
    {
        /// <summary>
        /// 为指定接口添加Password模式的token提供者
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <param name="services"></param>
        /// <param name="alias">TokenProvider的别名</param>
        /// <returns></returns>
        public static OptionsBuilder<PasswordCredentialsOptions> AddPasswordCredentialsTokenProvider<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] THttpApi>(
            this IServiceCollection services,
            string alias = "")
        {
            var builder = services.AddTokenProvider<THttpApi, PasswordCredentialsTokenProvider>(alias);
            return new OptionsBuilder<PasswordCredentialsOptions>(builder.Services, builder.Name);
        }

        /// <summary>
        /// 为指定接口添加Password模式的token提供者
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions">配置</param>
        /// <returns></returns>
        public static OptionsBuilder<PasswordCredentialsOptions> AddPasswordCredentialsTokenProvider<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] THttpApi>(
            this IServiceCollection services,
            Action<PasswordCredentialsOptions> configureOptions)
        {
            return services.AddPasswordCredentialsTokenProvider<THttpApi>().Configure(configureOptions);
        }

        /// <summary>
        /// 为指定接口添加Password模式的token提供者
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <param name="services"></param>
        /// <param name="alias">TokenProvider的别名</param>
        /// <param name="configureOptions">配置</param>
        /// <returns></returns>
        public static OptionsBuilder<PasswordCredentialsOptions> AddPasswordCredentialsTokenProvider<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] THttpApi>(
            this IServiceCollection services,
            string alias,
            Action<PasswordCredentialsOptions> configureOptions)
        {
            return services.AddPasswordCredentialsTokenProvider<THttpApi>(alias).Configure(configureOptions);
        }
    }
}