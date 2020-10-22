namespace WebApiClientCore.HttpMessageHandlers
{
    /// <summary>
    /// 设置授权原因
    /// </summary>
    public enum SetReason
    {
        /// <summary>
        /// 用于发送请求
        /// </summary>
        ForSend,

        /// <summary>
        /// 用于重试发送请求
        /// </summary>
        ForResend
    }
}
