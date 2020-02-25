using System;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// Define the behavior of the ApiAction filter
    /// </summary>
    public interface IApiActionFilter
    {
        /// <summary>
        /// Before preparing the request
        /// </summary>
        /// <param name="context">Context</param>
        /// <returns></returns>
        Task OnBeginRequestAsync(ApiActionContext context);

        /// <summary>
        /// After the request is completed
        /// </summary>
        /// <param name="context">Context</param>
        /// <returns></returns>
        Task OnEndRequestAsync(ApiActionContext context);
    }
}
