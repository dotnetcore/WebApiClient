using System.Reflection;

namespace WebApiClientCore.Defaults
{
    /// <summary>
    /// 默认的ApiAction描述器提供者
    /// </summary>
    public class ApiActionDescriptorProvider : IApiActionDescriptorProvider
    {
        /// <summary>
        /// 返回创建ApiActionDescriptor新实例
        /// </summary>
        /// <param name="method">接口的方法</param>
        /// <returns></returns>
        public virtual ApiActionDescriptor CreateApiActionDescriptor(MethodInfo method)
        {
            return new ApiActionDescriptor(method);
        }
    }
}
