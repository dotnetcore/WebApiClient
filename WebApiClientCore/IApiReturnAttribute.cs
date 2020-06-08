using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义回复内容处理特性的行为
    /// </summary>
    public interface IApiReturnAttribute : IAttributeMultiplable, IAttributeEnable
    {
        /// <summary>
        /// 获取期望的媒体类型
        /// </summary>
        MediaTypeWithQualityHeaderValue? AcceptContentType { get; }

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
