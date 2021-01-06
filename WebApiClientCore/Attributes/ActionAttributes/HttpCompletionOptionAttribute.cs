using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 指示请求完成选项的特性
    /// 缺省的情况下，当声明返回类型为Stream或HttpResponseMessage时使用ResponseHeadersRead
    /// </summary>
    public class HttpCompletionOptionAttribute : ApiActionAttribute
    {
        /// <summary>
        /// 请求完成选项
        /// </summary>
        private readonly HttpCompletionOption completionOption;

        /// <summary>
        /// 指示请求完成选项的特性
        /// </summary>
        /// <param name="completionOption">请求完成选项</param>
        public HttpCompletionOptionAttribute(HttpCompletionOption completionOption)
        {
            this.completionOption = completionOption;
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override Task OnRequestAsync(ApiRequestContext context)
        {
            context.HttpContext.CompletionOption = this.completionOption;
            return Task.CompletedTask;
        }
    }
}
