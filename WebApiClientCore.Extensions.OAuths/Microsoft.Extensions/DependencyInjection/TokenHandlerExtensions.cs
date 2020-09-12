using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
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
            return builder.AddOAuthTokenHandler((s, t) => new OAuthTokenHandler(s, t));
        }

        /// <summary>
        /// 添加token应用的http消息处理程序
        /// </summary>
        /// <typeparam name="TOAuthTokenHandler"></typeparam>
        /// <param name="builder"></param>
        /// <param name="handlerFactory">hanlder的创建委托</param> 
        /// <returns></returns>
        public static IHttpClientBuilder AddOAuthTokenHandler<TOAuthTokenHandler>(this IHttpClientBuilder builder, Func<IServiceProvider, Type, TOAuthTokenHandler> handlerFactory)
            where TOAuthTokenHandler : OAuthTokenHandler
        {
            var httpApiType = builder.GetHttpApiType();
            if (httpApiType == null)
            {
                return builder;
            }

            builder.Services.TryAddTransient(serviceProvider =>
            {
                return handlerFactory(serviceProvider, httpApiType);
            });

            return builder.AddHttpMessageHandler<TOAuthTokenHandler>();
        }
    }
}
