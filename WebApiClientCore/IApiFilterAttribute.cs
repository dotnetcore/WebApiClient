using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义ApiAction过滤器修饰特性的行为
    /// </summary>
    public interface IApiFilterAttribute :  IAttributeMultiplable
    {
        /// <summary>
      /// 准备请求之前
      /// </summary>
      /// <param name="context">上下文</param>
      /// <returns></returns>
        Task BeforeRequestAsync(ApiActionContext context);

        /// <summary>
        /// 请求完成之后
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        Task AfterRequestAsync(ApiActionContext context);
    }
}
