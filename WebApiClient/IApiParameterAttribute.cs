using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// Defining the behavior of Api parameter modification properties
    /// </summary>
    public interface IApiParameterAttribute
    {
        /// <summary>
        /// before http request
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="parameter">Parameters associated with characteristics</param>
        /// <returns></returns>
        Task BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter);
    }
}
