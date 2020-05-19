using Microsoft.Extensions.DependencyInjection;
using WebApiClientCore.Extensions.OAuths;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示client_credentials授权方式的token应用特性
    /// 需要先注册services.AddClientCredentialsTokenProvider
    /// </summary> 
    public class ClientCredentialsTokenAttribute : OAuthTokenAttribute
    {
        /// <summary>
        /// 获取token提供者
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected override TokenProvider GetTokenProvider(ApiRequestContext context)
        {
            var providerType = typeof(ClientCredentialsTokenProvider<>).MakeGenericType(context.ApiAction.InterfaceType);
            return (TokenProvider)context.HttpContext.Services.GetRequiredService(providerType);
        }
    }
}
