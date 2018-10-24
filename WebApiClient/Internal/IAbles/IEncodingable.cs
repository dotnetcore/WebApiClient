namespace WebApiClient
{
    /// <summary>
    /// 定义支持Encoding的配置
    /// </summary>
    interface IEncodingable
    {
        /// <summary>
        /// 获取或设置参数的编码名称
        /// </summary>
        string Encoding { get; set; }
    }
}
