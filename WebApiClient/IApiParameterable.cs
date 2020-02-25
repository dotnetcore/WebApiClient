using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// Defines the behavior of objects that can themselves be used as parameters and processed accordingly
    /// When this object is used as a parameter, no attribute modification is required
    /// </summary>
    public interface IApiParameterable
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
