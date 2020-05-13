using System.Threading.Tasks;

namespace WebApiClientCore
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
        /// <returns></returns>
        Task BeforeRequestAsync(ApiParameterContext context);
    }
}
