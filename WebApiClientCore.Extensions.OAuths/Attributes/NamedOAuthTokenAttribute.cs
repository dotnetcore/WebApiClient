using Microsoft.Extensions.DependencyInjection;
using System;
using WebApiClientCore.Extensions.OAuths;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示别名的token应用特性
    /// </summary>
    public class NamedOAuthTokenAttribute : OAuthTokenAttribute
    {
        /// <summary>
        /// 获取token提供者别名的参数名
        /// </summary>
        public string NameParameterName { get; }

        /// <summary>
        /// 别名的token应用特性
        /// </summary>
        /// <param name="nameParameterName">token提供者别名的参数名</param>
        public NamedOAuthTokenAttribute(string nameParameterName)
        {
            this.NameParameterName = nameParameterName;
        }

        /// <summary>
        /// <inheritdoc></inheritdoc>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        protected sealed override ITokenProvider GetTokenProvider(ApiRequestContext context)
        {
            if (context.TryGetArgument<string>(this.NameParameterName, StringComparer.OrdinalIgnoreCase, out var name))
            {
                var tokenProviderFactory = context.HttpContext.ServiceProvider.GetRequiredService<ITokenProviderFactory>();
                return tokenProviderFactory.Create(context.ActionDescriptor.InterfaceType, this.TokenProviderSearchMode, name);
            }

            throw new InvalidOperationException($"未提供有效的参数值: {this.NameParameterName}");
        }
    }
}
