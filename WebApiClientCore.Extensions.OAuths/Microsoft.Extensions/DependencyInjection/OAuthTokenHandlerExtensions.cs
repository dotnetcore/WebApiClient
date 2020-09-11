using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using WebApiClientCore.Extensions.OAuths.HttpMessageHandlers;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 提供OAuth授权token应用的http消息处理程序扩展
    /// </summary>
    public static class OAuthTokenHandlerExtensions
    {
        /// <summary>
        /// 添加client_credentials授权方式token应用的http消息处理程序
        /// 该功能等效于接口的[ClientCredentialsTokenAttribute]
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddClientCredentialsTokenHandler(this IHttpClientBuilder builder)
        {
            return builder.AddTokenHandler((s, t) => new ClientCredentialsTokenHandler(s, t));
        }

        /// <summary>
        /// 添加password授权方式token应用的http消息处理程序
        /// 该功能等效于接口的[PasswordCredentialsTokenAttribute]
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddPasswordCredentialsTokenHandler(this IHttpClientBuilder builder)
        {
            return builder.AddTokenHandler((s, t) => new PasswordCredentialsTokenHandler(s, t));
        }

        /// <summary>
        /// 添加token应用的http消息处理程序
        /// </summary>
        /// <typeparam name="TOAuthTokenHandler"></typeparam>
        /// <param name="builder"></param>
        /// <param name="handlerFactory">hanlder的创建委托</param> 
        /// <returns></returns>
        public static IHttpClientBuilder AddTokenHandler<TOAuthTokenHandler>(this IHttpClientBuilder builder, Func<IServiceProvider, Type, TOAuthTokenHandler> handlerFactory)
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
