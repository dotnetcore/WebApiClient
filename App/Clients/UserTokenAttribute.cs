using Microsoft.Extensions.DependencyInjection;
using WebApiClientCore;
using WebApiClientCore.Attributes;
using WebApiClientCore.Extensions.OAuths;

namespace App.Clients
{
    /// <summary>
    /// token获取与应用过滤器
    /// </summary>
    class UserTokenAttribute : OAuthTokenAttribute
    {
        /// <summary>
        /// 获取token提供者
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override TokenProvider GetTokenProvider(ApiRequestContext context)
        {
            return context.HttpContext.Services.GetRequiredService<ClientCredentialsTokenProvider<IUserApi>>();
        }

        /// <summary>
        /// 应用token
        /// </summary>
        /// <param name="context"></param>
        /// <param name="tokenResult"></param>
        protected override void UseTokenResult(ApiRequestContext context, TokenResult tokenResult)
        {
            base.UseTokenResult(context, tokenResult);
        }
    }
}
