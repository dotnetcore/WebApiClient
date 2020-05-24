namespace WebApiClientCore.Exceptions
{
    /// <summary>
    /// 表示Api的特性配置异常
    /// </summary>
    public class ApiInvalidOperationException : HttpApiException
    {
        /// <summary>
        /// Api的特性配置异常
        /// </summary>
        /// <param name="message">提示信息</param>
        public ApiInvalidOperationException(string? message) :
            base(message)
        {
        }
    }
}
