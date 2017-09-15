using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 定义ApiAction过滤器的行为
    /// </summary>
    public interface IApiActionFilterAttribute : IAttributeMultiplable
    {
        /// <summary>
        /// 获取顺序排序的索引
        /// </summary>
        int OrderIndex { get; }

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
    }
}
