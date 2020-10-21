namespace WebApiClientCore.HttpMessageHandlers
{
    /// <summary>
    /// 设置授权原因
    /// </summary>
    public enum AuthorizationReason
    {
        /// <summary>
        /// 用于发送
        /// </summary>
        SetForSend,

        /// <summary>
        /// 用于重试发送
        /// </summary>
        SetForResend
    }
}
