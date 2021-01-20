using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义Api过滤器的行为
    /// </summary>
    public interface IApiFilter
    {
        /// <summary>
        /// 请求前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        Task OnRequestAsync(ApiRequestContext context);

        /// <summary>
        /// 响应后
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        Task OnResponseAsync(ApiResponseContext context);
    }
}
