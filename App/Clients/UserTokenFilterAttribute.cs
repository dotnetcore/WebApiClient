using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WebApiClientCore;
using WebApiClientCore.Attributes;
using WebApiClientCore.OAuths;

namespace App.Clients
{
    /// <summary>
    /// token获取与应用过滤器
    /// </summary>
    class UserTokenFilterAttribute : ClientTokenFilterAttribue
    {
        protected override ClientCredentialsOptions GetClientCredentialsOptions(ApiRequestContext context)
        {
            var services = context.HttpContext.Services;
            var options = services.GetService<IOptions<ClientCredentialsOptions<IUserApi>>>();
            return options.Value;
        }
    }
}
