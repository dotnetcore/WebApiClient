namespace WebApiClientCore
{
    /// <summary>
    /// 定义特性是否启用的接口
    /// </summary>
    public interface IAttributeEnable
    {
        /// <summary>
        /// 获取特性是否启用
        /// </summary>
        bool Enable { get; }
    }
}
