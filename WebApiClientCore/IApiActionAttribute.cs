using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义ApiAction修饰特性的行为
    /// </summary>
    public interface IApiActionAttribute : IAttributeMultiplable
    {
        /// <summary>
        /// 请求前
        /// </summary>
        /// <param name="context">上下文</param> 
        /// <returns></returns>
        Task OnRequestAsync(ApiRequestContext context);
    }
}
