using System.Reflection;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义ApiAction描述器提供者的接口
    /// </summary>
    public interface IApiActionDescriptorProvider
    {
        /// <summary>
        /// 返回创建ApiActionDescriptor新实例
        /// </summary>
        /// <param name="method">接口的方法</param>
        /// <returns></returns>
        ApiActionDescriptor CreateApiActionDescriptor(MethodInfo method);
    }
}
