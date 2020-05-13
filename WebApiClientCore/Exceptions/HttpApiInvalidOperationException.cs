namespace WebApiClientCore.Exceptions
{
    /// <summary>
    /// 表示Http接口的特性配置异常
    /// </summary>
    public class HttpApiInvalidOperationException : HttpApiException
    {
        /// <summary>
        /// Http接口特性配置异常
        /// </summary>
        /// <param name="message">提示信息</param>
        public HttpApiInvalidOperationException(string message) :
            base(message)
        {
        }
    }
}
