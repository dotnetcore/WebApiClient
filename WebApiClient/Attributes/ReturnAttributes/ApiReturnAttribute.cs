using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 回复内容处理特性抽象
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class ApiReturnAttribute : Attribute, IApiReturnAttribute
    {
        /// <summary>
        /// 获取或设置是否确保响应的http状态码通过IsSuccessStatusCode验证
        /// 当设置为true之后，请求可能会引发HttpFailureStatusException
        /// </summary>
        public bool EnsureSuccessStatusCode { get; set; }

        /// <summary>
        /// 获取异步结果
        /// </summary>
        /// <param name="context">上下文</param>
        /// <exception cref="HttpFailureStatusException"></exception>
        /// <returns></returns>
        Task<object> IApiReturnAttribute.GetTaskResult(ApiActionContext context)
        {
            if (this.EnsureSuccessStatusCode == true)
            {
                var statusCode = context.ResponseMessage.StatusCode;
                if (this.IsSuccessStatusCode(statusCode) == false)
                {
                    var inner = new HttpRequestException($"响应的http状态码不成功：{(int)statusCode} {statusCode}");
                    throw new HttpFailureStatusException(statusCode, context, inner);
                }
            }
            return this.GetTaskResult(context);
        }


        /// <summary>
        /// 指示状态码是否为成功的状态码
        /// </summary>
        /// <param name="statusCode">状态码</param>
        /// <returns></returns>
        protected virtual bool IsSuccessStatusCode(HttpStatusCode statusCode)
        {
            var status = (int)statusCode;
            return status >= 200 & status <= 299;
        }

        /// <summary>
        /// 获取异步结果
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected abstract Task<object> GetTaskResult(ApiActionContext context);
    }
}
