namespace WebApiClientCore
{
    /// <summary>
    /// 定义ApiActionInvoker提供者的接口
    /// </summary>
    public interface IApiActionInvokerProvider
    {
        /// <summary>
        /// 创建Action执行器
        /// </summary>
        /// <param name="actionDescriptor">Action描述</param>
        /// <returns></returns>
        ApiActionInvoker CreateActionInvoker(ApiActionDescriptor actionDescriptor);
    }
}
