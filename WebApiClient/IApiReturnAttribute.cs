using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 定义回复内容处理特性的行为
    /// </summary>
    public interface IApiReturnAttribute
    {
        /// <summary>
        /// 获取异步结果
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        Task<object> GetTaskResult(ApiActionContext context);
    }
}
