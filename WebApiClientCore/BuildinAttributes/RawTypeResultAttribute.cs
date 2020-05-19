using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示原始值结果的特性
    /// 支持结果类型为string、byte[]、Stream和HttpResponseMessage
    /// </summary>
    class RawTypeResultAttribute : ApiResultAttribute
    {
        /// <summary>
        /// 原始值结果的特性
        /// </summary>
        public RawTypeResultAttribute()
            : base(null)
        {
        }

        /// <summary>
        /// 设置结果值
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override async Task SetResultAsync(ApiResponseContext context)
        {
            var response = context.HttpContext.ResponseMessage;
            var dataType = context.ApiAction.Return.DataType;

            if (dataType.IsHttpResponseMessage == true)
            {
                context.Result = response;
            }
            else if (dataType.IsString == true)
            {
                context.Result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            else if (dataType.IsByteArray == true)
            {
                context.Result = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            }
            else if (dataType.IsStream == true)
            {
                context.Result = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            }
        }
    }
}
