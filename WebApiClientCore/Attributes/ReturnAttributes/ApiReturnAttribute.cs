using System;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 回复内容处理特性抽象
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class ApiReturnAttribute : Attribute, IApiReturnAttribute
    {
        /// <summary>
        /// 获取或设置是否确保响应的http状态码通过IsSuccessStatusCode验证
        /// 当值为true时，请求可能会引发HttpStatusFailureException
        /// 默认为true
        /// </summary>
        public bool EnsureSuccessStatusCode { get; set; } = true;

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        Task IApiReturnAttribute.BeforeRequestAsync(ApiActionContext context)
        {
            this.ConfigureAccept(context.RequestMessage.Headers.Accept);
            return this.BeforeRequestAsync(context);
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected virtual Task BeforeRequestAsync(ApiActionContext context)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 配置请求头的accept
        /// </summary>
        /// <param name="accept">请求头的accept</param>
        /// <returns></returns>
        protected abstract void ConfigureAccept(HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> accept);

        /// <summary>
        /// 获取异步结果
        /// </summary>
        /// <param name="context">上下文</param>
        /// <exception cref="HttpStatusFailureException"></exception>
        /// <returns></returns>
        Task<object> IApiReturnAttribute.GetResultAsync(ApiActionContext context)
        {
            if (this.EnsureSuccessStatusCode == true)
            {
                var statusCode = context.ResponseMessage.StatusCode;
                if (this.IsSuccessStatusCode(statusCode) == false)
                {
                    throw new HttpStatusFailureException(context);
                }
            }
            return this.GetResultAsync(context);
        }


        /// <summary>
        /// 指示状态码是否为成功的状态码
        /// </summary>
        /// <param name="statusCode">状态码</param>
        /// <returns></returns>
        protected virtual bool IsSuccessStatusCode(HttpStatusCode statusCode)
        {
            var status = (int)statusCode;
            return status >= 200 && status <= 299;
        }

        /// <summary>
        /// 获取异步结果
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected abstract Task<object> GetResultAsync(ApiActionContext context);
    }
}
