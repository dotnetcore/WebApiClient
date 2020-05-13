namespace WebApiClientCore.Exceptions
{
    /// <summary>
    /// 表示请求Token异常
    /// </summary>
    public class HttpApiTokenException : HttpApiException
    {
        /// <summary>
        /// 请求Token异常
        /// </summary>
        /// <param name="message">消息</param>
        public HttpApiTokenException(string message)
            : base(message)
        {
        }
    }
}
