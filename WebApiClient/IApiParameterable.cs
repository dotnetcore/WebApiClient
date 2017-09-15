using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 定义可以做为参数并完成相应的处理的对象的行为
    /// </summary>
    public interface IApiParameterable
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        Task BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter);
    }
}
