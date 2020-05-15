using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义Api执行器的接口
    /// </summary>
    interface IApiActionInvoker
    {
        /// <summary>
        /// 执行Api方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task InvokeAsync(ApiRequestContext context);
    }
}
