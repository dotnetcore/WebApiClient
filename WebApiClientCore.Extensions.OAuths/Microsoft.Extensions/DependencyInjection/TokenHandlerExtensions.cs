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
        /// <param name="tokenProviderSearchMode">token提供者的查找模式</param>
        /// <returns></returns>
        public static IHttpClientBuilder AddOAuthTokenHandler(this IHttpClientBuilder builder, TypeMatchMode tokenProviderSearchMode = TypeMatchMode.TypeOrBaseTypes)
        {
            return builder.AddOAuthTokenHandler((s, t) => new OAuthTokenHandler(t), tokenProviderSearchMode);
        }

        /// <summary>
        /// 添加token应用的http消息处理程序
        /// </summary>
        /// <typeparam name="TOAuthTokenHandler"></typeparam>
        /// <param name="builder"></param>
        /// <param name="handlerFactory">hanlder的创建委托</param>
        /// <param name="tokenProviderSearchMode">token提供者的查找模式</param> 
        /// <returns></returns>
        public static IHttpClientBuilder AddOAuthTokenHandler<TOAuthTokenHandler>(this IHttpClientBuilder builder, Func<IServiceProvider, ITokenProvider, TOAuthTokenHandler> handlerFactory, TypeMatchMode tokenProviderSearchMode = TypeMatchMode.TypeOrBaseTypes)
            where TOAuthTokenHandler : OAuthTokenHandler
        {
            var httpApiType = builder.GetHttpApiType();
            if (httpApiType == null)
            {
                throw new InvalidOperationException($"无效的{nameof(IHttpClientBuilder)}，找不到其关联的http接口类型");
            }

            builder.Services.TryAddTransient(serviceProvider =>
            {
                var factory = serviceProvider.GetRequiredService<ITokenProviderFactory>();
                var tokenProvider = factory.Create(httpApiType, tokenProviderSearchMode);
                return handlerFactory(serviceProvider, tokenProvider);
            });

            return builder.AddHttpMessageHandler<TOAuthTokenHandler>();
        }
    }
}
