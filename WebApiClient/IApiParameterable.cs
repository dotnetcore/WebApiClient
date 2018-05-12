using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 定义自身可以做为参数并进行相应处理的对象的行为
    /// 此对象作为参数时，不需要特性修饰
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
