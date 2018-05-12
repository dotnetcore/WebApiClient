using System;

namespace WebApiClient
{
    /// <summary>
    /// 表示Http接口的特性配置异常
    /// </summary>
    public class HttpApiConfigException : Exception
    {
        /// <summary>
        /// Http接口特性配置异常
        /// </summary>
        /// <param name="message">提示信息</param>
        public HttpApiConfigException(string message) :
            base(message)
        {
        }
    }
}
