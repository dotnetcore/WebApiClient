using Microsoft.Extensions.DependencyInjection;
using WebApiClientCore.Exceptions;
using WebApiClientCore.Extensions.OAuths;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示由client模式token提供者提供的token应用特性
    /// 需要注册services.AddClientCredentialsTokenProvider
    /// </summary> 
    public class ClientCredentialsTokenAttribute : OAuthTokenAttribute
    {
        /// <summary>
        /// 获取token提供者
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override ITokenProvider GetTokenProvider(ApiRequestContext context)
        {
            var provider = base.GetTokenProvider(context);
            if (provider.ProviderType != ProviderType.ClientCredentials)
            {
                throw new ApiInvalidConfigException($"未注册{nameof(TokenProviderExtensions.AddClientCredentialsTokenProvider)}");
            }
            return provider;
        }
    }
}
