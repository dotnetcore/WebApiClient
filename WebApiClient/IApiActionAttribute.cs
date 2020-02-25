using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// Define the behavior of the ApiAction modifier
    /// </summary>
    public interface IApiActionAttribute : IAttributeMultiplable
    {
        /// <summary>
        /// Before execution
        /// </summary>
        /// <param name="context">Context</param>
        /// <returns></returns>
        Task BeforeRequestAsync(ApiActionContext context);
    }
}
