using Microsoft.Extensions.DependencyInjection;
using System;
using WebApiClientCore.Extensions.OAuths;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示别名的token应用特性
    /// </summary>
    public class AliasOAuthTokenAttribute : OAuthTokenAttribute
    {
        /// <summary>
        /// 获取TokenProvider别名的参数名
        /// </summary>
        public string AliasParameterName { get; }

        /// <summary>
        /// 别名的token应用特性
        /// </summary>
        /// <param name="aliasParameterName">TokenProvider别名的参数名</param>
        public AliasOAuthTokenAttribute(string aliasParameterName)
        {
            this.AliasParameterName = aliasParameterName;
        }

        /// <summary>
        /// <inheritdoc></inheritdoc>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        protected sealed override ITokenProvider GetTokenProvider(ApiRequestContext context)
        {
            if (context.TryGetArgument<string>(this.AliasParameterName, StringComparer.OrdinalIgnoreCase, out var alias))
            {
                var tokenProviderFactory = context.HttpContext.ServiceProvider.GetRequiredService<ITokenProviderFactory>();
                return tokenProviderFactory.Create(context.ActionDescriptor.InterfaceType, this.TokenProviderSearchMode, alias);
            }

            throw new InvalidOperationException($"未提供有效的参数值: {this.AliasParameterName}");
        }
    }
}
