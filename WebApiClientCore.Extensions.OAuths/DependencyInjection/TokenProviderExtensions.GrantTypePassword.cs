using Microsoft.Extensions.Options;
using System;
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
        /// <returns></returns>
        public static OptionsBuilder<PasswordCredentialsOptions> AddPasswordCredentialsTokenProvider<THttpApi>(this IServiceCollection services)
        {
            var builder = services.AddTokenProvider<THttpApi, PasswordCredentialsTokenProvider>();
            return new OptionsBuilder<PasswordCredentialsOptions>(builder.Services, builder.Name);
        }

        /// <summary>
        /// 为指定接口添加Password模式的token提供者
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions">配置</param>
        /// <returns></returns>
        public static OptionsBuilder<PasswordCredentialsOptions> AddPasswordCredentialsTokenProvider<THttpApi>(this IServiceCollection services, Action<PasswordCredentialsOptions> configureOptions)
        {
            return services.AddPasswordCredentialsTokenProvider<THttpApi>().Configure(configureOptions);
        }
    }
}