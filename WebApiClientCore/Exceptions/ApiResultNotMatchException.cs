namespace WebApiClientCore.Exceptions
{
    /// <summary>
    /// 表示接口的结果不匹配异常
    /// </summary>
    public class ApiResultNotMatchException : ApiException
    {
        /// <summary>
        /// 获取结果值
        /// </summary>
        public object? Result { get; }

        /// <summary>
        /// 接口的结果不匹配异常
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="result">结果值</param>
        public ApiResultNotMatchException(string? message, object? result)
            : base(message)
        {
            this.Result = result;
        }
    }
}
