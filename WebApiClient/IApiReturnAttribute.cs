using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// Defining the behavior of response content processing features
    /// </summary>
    public interface IApiReturnAttribute
    {
        /// <summary>
        /// Before execution
        /// </summary>
        /// <param name="context">Context</param>
        /// <returns></returns>
        Task BeforeRequestAsync(ApiActionContext context);

        /// <summary>
        /// 执行后获取异步结果
        /// </summary>
        /// <param name="context">Get asynchronous results after execution</param>
        /// <returns></returns>
        Task<object> GetTaskResult(ApiActionContext context);
    }
}
