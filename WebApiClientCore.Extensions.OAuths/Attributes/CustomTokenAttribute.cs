using Microsoft.Extensions.DependencyInjection;
using WebApiClientCore.Exceptions;
using WebApiClientCore.Extensions.OAuths;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示由自定义token提供者提供的token应用特性
    /// 需要注册services.AddCustomTokenProvider
    /// </summary> 
    public class CustomTokenAttribute : OAuthTokenAttribute
    {
        /// <summary>
        /// 获取token提供者
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override ITokenProvider GetTokenProvider(ApiRequestContext context)
        {
            var provider = base.GetTokenProvider(context);
            if (provider.ProviderType != ProviderType.Custom)
            {
                throw new ApiInvalidConfigException($"未注册{nameof(TokenProviderExtensions.AddCustomTokenProvider)}");
            }
            return provider;
        }
    }
}
