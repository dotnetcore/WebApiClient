using Microsoft.Extensions.DependencyInjection;
using WebApiClientCore;
using WebApiClientCore.Attributes;
using WebApiClientCore.OAuths;

namespace App.Clients
{
    /// <summary>
    /// token获取与应用过滤器
    /// </summary>
    class UserTokenFilterAttribute : TokenFilterAttribute
    {
        protected override TokenProvider GetTokenProvider(ApiRequestContext context)
        {
            return context.HttpContext.Services.GetRequiredService<ClientCredentialsTokenProvider<IUserApi>>();
        } 
    }
}
