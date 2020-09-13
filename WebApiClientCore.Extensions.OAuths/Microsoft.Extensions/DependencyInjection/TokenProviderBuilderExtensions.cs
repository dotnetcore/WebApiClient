using Microsoft.Extensions.Options;
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
        /// <returns></returns>
        public static OptionsBuilder<ClientCredentialsOptions> AddClientCredentialsOptions(this ITokenProviderBuilder builder)
        {
            return builder.AddOptions<ClientCredentialsOptions>();
        }


        /// <summary>
        /// 配置Password模式的授权信息
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static OptionsBuilder<PasswordCredentialsOptions> AddPasswordCredentialsOptions(this ITokenProviderBuilder builder)
        {
            return builder.AddOptions<PasswordCredentialsOptions>();
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
