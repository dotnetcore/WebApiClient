namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 定义支持集合序列化方式的接口
    /// </summary>
    interface ICollectionFormatable
    {
        /// <summary>
        /// 获取或设置集合格式化方式
        /// </summary>
        CollectionFormat CollectionFormat { get; set; }
    }
}
