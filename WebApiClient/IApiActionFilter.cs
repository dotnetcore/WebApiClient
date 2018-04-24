using System;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 定义ApiAction过滤器的行为
    /// </summary>
    public interface IApiActionFilter
    {
        /// <summary>
        /// 准备请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        Task OnBeginRequestAsync(ApiActionContext context);

        /// <summary>
        /// 请求完成之后
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        Task OnEndRequestAsync(ApiActionContext context);
        
        /// <summary>
        /// 执行异常时
        /// 返回true终止传递异常给下一下过滤器
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="exception"></param>
        /// <returns></returns>
        Task<bool> OnExceptionAsync(ApiActionContext context, Exception exception);
    }
}
