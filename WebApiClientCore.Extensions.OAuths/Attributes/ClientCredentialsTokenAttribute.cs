using Microsoft.Extensions.DependencyInjection;
using WebApiClientCore.Extensions.OAuths;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示client_credentials授权方式的token应用特性
    /// 需要注册services.AddClientCredentialsTokenProvider
    /// </summary> 
    public class ClientCredentialsTokenAttribute : OAuthTokenAttribute
    {
        /// <summary>
        /// 获取token提供者
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected override ITokenProvider GetTokenProvider(ApiRequestContext context)
        {
            var providerType = typeof(IClientCredentialsTokenProvider<>).MakeGenericType(context.ApiAction.InterfaceType);
            return (ITokenProvider)context.HttpContext.ServiceProvider.GetRequiredService(providerType);
        }
    }
}
