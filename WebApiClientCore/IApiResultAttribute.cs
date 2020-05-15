using System;
using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义回复内容处理特性的行为
    /// </summary>
    public interface IApiResultAttribute : IAttributeMultiplable
    {
        /// <summary>
        /// 请求前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="next">下一个执行委托</param>
        /// <returns></returns>
        Task BeforeRequestAsync(ApiActionContext context, Func<Task> next);

        /// <summary>
        /// 请求后
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="next">下一个执行委托</param>
        /// <returns></returns>
        Task AfterRequestAsync(ApiActionContext context, Func<Task> next);
    }
}
