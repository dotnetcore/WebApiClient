namespace WebApiClient
{
    /// <summary>
    /// 定义支持DateTimeFormat的配置
    /// </summary>
    interface IDateTimeFormatable
    {
        /// <summary>
        /// 获取或设置时期时间格式
        /// </summary>
        string DateTimeFormat { get; set; }
    }
}
