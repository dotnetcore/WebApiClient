using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 定义Api参数修饰特性的行为
    /// </summary>
    public interface IApiParameterAttribute
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
