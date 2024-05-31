using Microsoft.Extensions.DependencyInjection;
using System;
using WebApiClientCore.Extensions.OAuths;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示支持多用户的token应用特性
    /// </summary>
    public class MultiUserOAuthTokenAttribute : OAuthTokenAttribute
    {
        /// <summary>
        /// 获取string类型的用户Id参数名
        /// </summary>
        public string UserIdParameterName { get; }

        /// <summary>
        /// 支持多用户的token应用特性
        /// </summary>
        /// <param name="userIdParameterName">string类型的用户Id参数名</param>
        public MultiUserOAuthTokenAttribute(string userIdParameterName)
        {
            this.UserIdParameterName = userIdParameterName;
        }

        /// <summary>
        /// <inheritdoc></inheritdoc>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        protected sealed override ITokenProvider GetTokenProvider(ApiRequestContext context)
        {
            if (context.TryGetArgument<string>(this.UserIdParameterName, StringComparer.OrdinalIgnoreCase, out var userId))
            {
                var tokenProviderFactory = context.HttpContext.ServiceProvider.GetRequiredService<ITokenProviderFactory>();
                return tokenProviderFactory.Create(context.ActionDescriptor.InterfaceType, this.TokenProviderSearchMode, userId);
            }

            throw new InvalidOperationException($"未提供有效的参数值: {this.UserIdParameterName}");
        }
    }
}
