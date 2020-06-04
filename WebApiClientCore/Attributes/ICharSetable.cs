namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 定义支持自定义编码的配置
    /// </summary>
    interface ICharSetable
    {
        /// <summary>
        /// 获取或设置编码名称
        /// </summary>
        string CharSet { get; set; }
    }
}
