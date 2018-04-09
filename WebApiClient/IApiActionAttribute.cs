using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 定义ApiAction修饰特性的行为
    /// </summary>
    public interface IApiActionAttribute : IAttributeMultiplable
    {
        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        Task BeforeRequestAsync(ApiActionContext context);
    }
}
