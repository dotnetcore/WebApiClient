using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义自身可以做为参数并进行相应处理的对象的行为
    /// 此对象作为参数时，不需要特性修饰
    /// </summary>
    public interface IApiParameter
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param> 
        /// <returns></returns>
        Task OnRequestAsync(ApiParameterContext context);
    }
}
