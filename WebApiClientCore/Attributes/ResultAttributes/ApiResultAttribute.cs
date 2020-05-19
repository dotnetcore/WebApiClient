using System;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示响应内容处理的抽象特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class ApiResultAttribute : Attribute, IApiResultAttribute
    {
        /// <summary>
        /// 获取接受的媒体类型
        /// </summary>
        protected MediaTypeWithQualityHeaderValue AcceptContentType { get; }

        /// <summary>
        /// 获取执行排序索引
        /// 默认通过Accept的Quality转换得到
        /// </summary>
        public virtual int OrderIndex
        {
            get
            {
                var quality = this.AcceptContentType?.Quality ?? 1d;
                return (int)((1d - quality) * int.MaxValue);
            }
        }

        /// <summary>
        /// 获取或设置是否启用
        /// </summary>
        public bool Enable { get; set; } = true;

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
        /// 获取或设置是否确保响应的ContentType与指定的Accpet-ContentType一致
        /// 默认为true
        /// </summary>
        public bool EnsureMatchAcceptContentType { get; set; } = true;

        /// <summary>
        /// 响应内容处理的抽象特性
        /// </summary>
        /// <param name="accpetContentType">收受的内容类型</param>
        public ApiResultAttribute(MediaTypeWithQualityHeaderValue accpetContentType)
        {
            this.AcceptContentType = accpetContentType;
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="next"></param>
        /// <returns></returns>
        public Task OnRequestAsync(ApiRequestContext context, Func<Task> next)
        {
            if (this.Enable == true && this.AcceptContentType != null)
            {
                context.HttpContext.RequestMessage.Headers.Accept.Add(this.AcceptContentType);
            }
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
            if (this.Enable &&
                context.ResultStatus == ResultStatus.None &&
                this.UseSuccessStatusCode(context) &&
                this.UseMatchAcceptContentType(context))
            {
                try
                {
                    await this.SetResultAsync(context).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    context.Exception = ex;
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
        /// 应用匹配ContentType
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool UseMatchAcceptContentType(ApiResponseContext context)
        {
            if (this.EnsureMatchAcceptContentType == true && this.AcceptContentType != null)
            {
                var contenType = context.HttpContext.ResponseMessage.Content.Headers.ContentType;
                if (this.IsMatchAcceptContentType(contenType) == false)
                {
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
        /// 验证响应的ContentType与Accept-ContentType是否匹配
        /// </summary>
        /// <param name="responseContentType"></param>
        /// <returns></returns>
        protected virtual bool IsMatchAcceptContentType(MediaTypeHeaderValue responseContentType)
        {
            var accept = this.AcceptContentType.MediaType;
            var response = responseContentType?.MediaType;
            return string.Equals(accept, response, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 设置结果值
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract Task SetResultAsync(ApiResponseContext context);
    }
}
