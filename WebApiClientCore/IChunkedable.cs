namespace WebApiClientCore
{
    /// <summary>
    /// 是否允许chunked传输
    /// </summary>
    interface IChunkedable
    {
        /// <summary>
        /// 获取或设置是否允许 chunked 传输
        /// </summary>
        bool AllowChunked { get; set; }
    }
}
