namespace WebApiClientCore
{
    /// <summary>
    /// 表示结果状态
    /// </summary>
    public enum ResultStatus : byte
    {
        /// <summary>
        /// 无状态
        /// </summary>
        None,

        /// <summary>
        /// 有结果
        /// </summary>
        HasResult,

        /// <summary>
        /// 有异常
        /// </summary>
        HasException,
    }
}
