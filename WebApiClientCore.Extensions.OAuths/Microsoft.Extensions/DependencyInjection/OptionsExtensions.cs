using Microsoft.Extensions.Options;
using System;
using WebApiClientCore;
using WebApiClientCore.Extensions.OAuths;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Options扩展
    /// </summary>
    public static class OptionsExtensions
    {
        /// <summary>
        /// 为接口配置ClientCredentialsOptions
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static OptionsBuilder<ClientCredentialsOptions> AddClientCredentialsOptions<THttpApi>(this IServiceCollection services)
        {
            return services.AddClientCredentialsOptions(typeof(THttpApi));
        }

        /// <summary>
        /// 为接口配置ClientCredentialsOptions
        /// </summary>
        /// <param name="services"></param>
        /// <param name="httpApiType">接口类型</param>
        /// <returns></returns>
        public static OptionsBuilder<ClientCredentialsOptions> AddClientCredentialsOptions(this IServiceCollection services, Type httpApiType)
        {
            var name = HttpApi.GetName(httpApiType);
            return services.AddOptions<ClientCredentialsOptions>(name);
        }

        /// <summary>
        /// 为接口配置PasswordCredentialsOptions
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static OptionsBuilder<PasswordCredentialsOptions> AddPasswordCredentialsOptions<THttpApi>(this IServiceCollection services)
        {
            return services.AddPasswordCredentialsOptions(typeof(THttpApi));
        }

        /// <summary>
        /// 为接口配置PasswordCredentialsOptions
        /// </summary>
        /// <param name="services"></param>
        /// <param name="httpApiType">接口类型</param>
        /// <returns></returns>
        public static OptionsBuilder<PasswordCredentialsOptions> AddPasswordCredentialsOptions(this IServiceCollection services, Type httpApiType)
        {
            var name = HttpApi.GetName(httpApiType);
            return services.AddOptions<PasswordCredentialsOptions>(name);
        }
    }
}
