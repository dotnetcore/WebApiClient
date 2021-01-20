namespace WebApiClientCore
{
    /// <summary>
    /// 定义特性是否允许在接口与方法上重复声明
    /// 如果不允许则优先选取方法上的特性
    /// </summary>
    public interface IAttributeMultiplable
    {
        /// <summary>
        /// 获取顺序排序的索引
        /// </summary>
        int OrderIndex { get; }

        /// <summary>
        /// 获取本类型是否允许在接口与方法上重复
        /// </summary>
        bool AllowMultiple { get; }
    }
}
