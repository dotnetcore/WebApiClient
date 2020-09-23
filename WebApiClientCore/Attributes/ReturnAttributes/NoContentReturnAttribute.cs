using System.Net;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示无内容的结果特性
    /// </summary> 
    public sealed class NoContentReturnAttribute : SpecialReturnAttribute
    {
        /// <summary>
        /// 无内容的结果特性
        /// </summary>
        public NoContentReturnAttribute()
            : base()
        {
        }

        /// <summary>
        /// 无内容的结果特性
        /// </summary>
        /// <param name="acceptQuality">accept的质比</param>
        public NoContentReturnAttribute(double acceptQuality)
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
            if (context.ApiAction.Return.DataType.IsRawType == false)
            {
                var response = context.HttpContext.ResponseMessage;
                if (response != null && response.StatusCode == HttpStatusCode.NoContent)
                {
                    context.Result = context.ApiAction.Return.DataType.Type.DefaultValue();
                }
            }

            return Task.CompletedTask;
        }
    }
}
