using System.Net;

namespace WebApiClientCore.Exceptions
{
    /// <summary>
    /// 可获取StatusCode的异常
    /// </summary>
    interface IStatusCodeException
    {
        /// <summary>
        /// 获取响应状态码
        /// </summary>
        HttpStatusCode? GetStatusCode();
    }
}
