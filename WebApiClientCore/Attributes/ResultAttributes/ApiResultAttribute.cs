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
    public abstract class ApiResultAttribute : Attribute, IApiResultAttribute
    {
        /// <summary>
        /// 获取或设置顺序排序的索引
        /// </summary>
        public int OrderIndex { get; set; }

        /// <summary>
        /// 获取本类型是否允许在接口与方法上重复
        /// </summary>
        public bool AllowMultiple => this.GetType().IsAllowMultiple();

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
        /// <param name="next"></param>
        /// <returns></returns>
        public Task OnRequestAsync(ApiRequestContext context, Func<Task> next)
        {
            this.ConfigureAccept(context.HttpContext.RequestMessage.Headers.Accept);
            return next();
        }

        /// <summary>
        /// 响应后
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="next">下一个执行委托</param>
        /// <returns></returns>
        public async Task OnResponseAsync(ApiResponseContext context, Func<Task> next)
        {
            if (context.ResultStatus == ResultStatus.None)
            {
                if (this.UseSuccessStatusCode(context) == true)
                {
                    try
                    {
                        await this.SetResultAsync(context);
                    }
                    catch (Exception ex)
                    {
                        context.Exception = ex;
                    }
                }
            }

            await next();
        }

        /// <summary>
        /// 应用成功状态码
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool UseSuccessStatusCode(ApiResponseContext context)
        {
            if (this.EnsureSuccessStatusCode == true)
            {
                var statusCode = context.HttpContext.ResponseMessage.StatusCode;
                if (this.IsSuccessStatusCode(statusCode) == false)
                {
                    context.Exception = new HttpStatusFailureException(context);
                    return false;
                }
            }
            return true;
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
        /// 配置请求头的accept
        /// </summary>
        /// <param name="accept">请求头的accept</param>
        public abstract void ConfigureAccept(HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> accept);

        /// <summary>
        /// 设置结果值
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract Task SetResultAsync(ApiResponseContext context);
    }
}
