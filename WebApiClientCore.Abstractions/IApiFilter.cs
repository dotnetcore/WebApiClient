using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义Api过滤器的行为
    /// </summary>
    public interface IApiFilter
    {
        /// <summary>
        /// 执行请求
        /// TODO 是否要增加新接口？而不是接口默认实现
        /// </summary>
        /// <param name="request">请求上下文</param>
        /// <param name="next">请求委托</param>
        /// <returns></returns>
        async Task<ApiResponseContext> ExecuteAsync(ApiRequestContext request, RequestDelegate next)
        {
            await this.OnRequestAsync(request).ConfigureAwait(false);
            var response = await next(request).ConfigureAwait(false);
            await this.OnResponseAsync(response).ConfigureAwait(false);
            return response;
        }

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
