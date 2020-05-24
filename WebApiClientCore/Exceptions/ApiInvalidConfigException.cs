namespace WebApiClientCore.Exceptions
{
    /// <summary>
    /// 表示接口无效的配置异常
    /// </summary>
    public class ApiInvalidConfigException : ApiException
    {
        /// <summary>
        /// 接口无效的配置异常
        /// </summary>
        /// <param name="message">提示信息</param>
        public ApiInvalidConfigException(string? message) :
            base(message)
        {
        }
    }
}
