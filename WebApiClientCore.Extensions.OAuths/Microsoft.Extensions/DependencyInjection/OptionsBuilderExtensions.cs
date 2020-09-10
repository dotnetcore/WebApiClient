using Microsoft.Extensions.Options;
using System;
using WebApiClientCore;
using WebApiClientCore.Extensions.OAuths;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 提供OptionsBuilder扩展
    /// </summary>
    public static class OptionsBuilderExtensions
    {
        /// <summary>
        /// 为接口配置ClientCredentialsOptions
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static OptionsBuilder<ClientCredentialsOptions> AddOptions<THttpApi>(this IClientCredentialsOptionsBuilder builder)
        {
            return builder.AddOptions(typeof(THttpApi));
        }

        /// <summary>
        /// 为接口配置ClientCredentialsOptions
        /// </summary> 
        /// <param name="builder"></param>
        /// <param name="httpApiType">接口类型</param>
        /// <returns></returns>
        public static OptionsBuilder<ClientCredentialsOptions> AddOptions(this IClientCredentialsOptionsBuilder builder, Type httpApiType)
        {
            var name = HttpApi.GetName(httpApiType);
            return builder.Services.AddOptions<ClientCredentialsOptions>(name);
        }


        /// <summary>
        /// 为接口配置PasswordCredentialsOptions
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static OptionsBuilder<PasswordCredentialsOptions> AddOptions<THttpApi>(this IPasswordCredentialsOptionsBuilder builder)
        {
            return builder.AddOptions(typeof(THttpApi));
        }

        /// <summary>
        /// 为接口配置PasswordCredentialsOptions
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="httpApiType">接口类型</param>
        /// <returns></returns>
        public static OptionsBuilder<PasswordCredentialsOptions> AddOptions(this IPasswordCredentialsOptionsBuilder builder, Type httpApiType)
        {
            var name = HttpApi.GetName(httpApiType);
            return builder.Services.AddOptions<PasswordCredentialsOptions>(name);
        }
    }
}
