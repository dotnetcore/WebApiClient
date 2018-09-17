namespace WebApiClient
{
    /// <summary>
    /// 定义支持IgnoreWhenNull的配置
    /// </summary>
    interface IIgnoreWhenNullable
    {
        /// <summary>
        /// 获取或设置当值为null是否忽略提交
        /// 默认为false
        /// </summary>
        bool IgnoreWhenNull { get; set; }
    }
}
