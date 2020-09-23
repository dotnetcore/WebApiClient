using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示无内容的结果特性
    /// 将结果设置为类型的默认值
    /// </summary> 
    public sealed class NoneReturnAttribute : SpecialReturnAttribute
    {
        /// <summary>
        /// 无内容的结果特性
        /// </summary>
        public NoneReturnAttribute()
            : base()
        {
        }

        /// <summary>
        /// 无内容的结果特性
        /// </summary>
        /// <param name="acceptQuality">accept的质比</param>
        public NoneReturnAttribute(double acceptQuality)
            : base(acceptQuality)
        {
        }

        /// <summary>
        /// 响应后
        /// </summary>
        /// <param name="context">上下文</param> 
        /// <returns></returns>
        public override Task OnResponseAsync(ApiResponseContext context)
        {
            if (context.ApiAction.Return.DataType.IsRawType == true)
            {
                return Task.CompletedTask;
            }

            var response = context.HttpContext.ResponseMessage;
            if (response != null && IsNoContent(response))
            {
                context.Result = context.ApiAction.Return.DataType.Type.DefaultValue();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 指示是否为无内容
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private static bool IsNoContent(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return true;
            }

            var status = (int)response.StatusCode;
            return status >= 200 && status <= 299 && response.Content.Headers.ContentLength == 0;
        }
    }
}
