using System;
using System.Diagnostics.CodeAnalysis;
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
        /// 需要为接口或接口的基础接口注册TokenProvider
        /// </summary>
        /// <remarks>
        /// <para>• Client模式：services.AddClientCredentialsTokenProvider</para>
        /// <para>• Password模式：services.AddPasswordCredentialsTokenProvider</para>
        /// </remarks>
        /// <param name="builder"></param>
        /// <param name="tokenProviderSearchMode">token提供者的查找模式</param>
        /// <returns></returns>
        public static IHttpClientBuilder AddOAuthTokenHandler(this IHttpClientBuilder builder, TypeMatchMode tokenProviderSearchMode = TypeMatchMode.TypeOrBaseTypes)
        {
            return builder.AddOAuthTokenHandler((s, t) => new OAuthTokenHandler(t), tokenProviderSearchMode);
        }

        /// <summary>
        /// 添加token应用的http消息处理程序
        /// 需要为接口或接口的基础接口注册TokenProvider
        /// </summary>
        /// <remarks>
        /// <para>• Client模式：services.AddClientCredentialsTokenProvider</para>
        /// <para>• Password模式：services.AddPasswordCredentialsTokenProvider</para>
        /// </remarks>
        /// <typeparam name="TOAuthTokenHandler"></typeparam>
        /// <param name="builder"></param>
        /// <param name="handlerFactory">hanlder的创建委托</param>
        /// <param name="tokenProviderSearchMode">token提供者的查找模式</param> 
        /// <returns></returns>
#if NET5_0_OR_GREATER
        [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2072", Justification = "类型httpApiType明确是不会被裁剪的")]
#endif
        public static IHttpClientBuilder AddOAuthTokenHandler<TOAuthTokenHandler>(this IHttpClientBuilder builder, Func<IServiceProvider, ITokenProvider, TOAuthTokenHandler> handlerFactory, TypeMatchMode tokenProviderSearchMode = TypeMatchMode.TypeOrBaseTypes)
            where TOAuthTokenHandler : OAuthTokenHandler
        {
            var httpApiType = builder.GetHttpApiType();
            if (httpApiType == null)
            {
                throw new InvalidOperationException($"无效的{nameof(IHttpClientBuilder)}，找不到其关联的http接口类型");
            }

            return builder.AddHttpMessageHandler(serviceProvider =>
            {
                var factory = serviceProvider.GetRequiredService<ITokenProviderFactory>();
                var tokenProvider = factory.Create(httpApiType, tokenProviderSearchMode);
                return handlerFactory(serviceProvider, tokenProvider);
            });
        }
    }
}
