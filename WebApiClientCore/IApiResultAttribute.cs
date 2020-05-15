using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义回复内容处理特性的行为
    /// </summary>
    public interface IApiResultAttribute
    {
        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        Task BeforeRequestAsync(ApiActionContext context);

        /// <summary>
        /// 执行后获取异步结果
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        Task<object> GetResultAsync(ApiActionContext context);
    }
}
