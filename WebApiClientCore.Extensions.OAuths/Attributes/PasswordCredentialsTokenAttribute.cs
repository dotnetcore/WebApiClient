using Microsoft.Extensions.DependencyInjection;
using WebApiClientCore.Extensions.OAuths;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示password授权方式的token应用特性
    /// 需要先注册services.AddPasswordCredentialsTokenProvider
    /// </summary> 
    public class PasswordCredentialsTokenAttribute : OAuthTokenAttribute
    {
        /// <summary>
        /// 获取token提供者
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected override ITokenProvider GetTokenProvider(ApiRequestContext context)
        {
            var providerType = typeof(IPasswordCredentialsTokenProvider<>).MakeGenericType(context.ApiAction.InterfaceType);
            return (ITokenProvider)context.HttpContext.ServiceProvider.GetRequiredService(providerType);
        }
    }
}
