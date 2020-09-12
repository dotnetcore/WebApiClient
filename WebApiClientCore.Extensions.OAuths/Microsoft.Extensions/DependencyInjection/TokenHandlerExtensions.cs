using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using WebApiClientCore.Extensions.OAuths;
using WebApiClientCore.Extensions.OAuths.HttpMessageHandlers;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 提供OAuth授权token应用的http消息处理程序扩展
    /// </summary>
    public static class TokenHandlerExtensions
    {
        /// <summary>
        /// 添加token应用的http消息处理程序
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddOAuthTokenHandler(this IHttpClientBuilder builder)
        {
            return builder.AddOAuthTokenHandler((s, t) => new OAuthTokenHandler(t));
        }

        /// <summary>
        /// 添加token应用的http消息处理程序
        /// </summary>
        /// <typeparam name="TOAuthTokenHandler"></typeparam>
        /// <param name="builder"></param>
        /// <param name="handlerFactory">hanlder的创建委托</param> 
        /// <returns></returns>
        public static IHttpClientBuilder AddOAuthTokenHandler<TOAuthTokenHandler>(this IHttpClientBuilder builder, Func<IServiceProvider, ITokenProvider, TOAuthTokenHandler> handlerFactory)
            where TOAuthTokenHandler : OAuthTokenHandler
        {
            builder.Services.TryAddTransient(serviceProvider =>
            {
                var tokenProvider = serviceProvider.GetRequiredService<ITokenProviderFactory>().Create(builder.Name);
                return handlerFactory(serviceProvider, tokenProvider);
            });

            return builder.AddHttpMessageHandler<TOAuthTokenHandler>();
        }
    }
}
