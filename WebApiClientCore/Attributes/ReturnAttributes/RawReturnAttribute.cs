using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示原始类型的结果特性
    /// 支持结果类型为string、byte[]、Stream和HttpResponseMessage
    /// </summary> 
    public sealed class RawReturnAttribute : SpecialReturnAttribute
    {
        /// <summary>
        /// 获取或设置是否确保响应的http状态码通过IsSuccessStatusCode验证
        /// 当值为true时，请求可能会引发HttpStatusFailureException
        /// 默认为true
        /// </summary>
        public bool EnsureSuccessStatusCode { get; set; } = true;

        /// <summary>
        /// 原始类型的结果特性
        /// </summary>
        public RawReturnAttribute()
            : base()
        {
        }

        /// <summary>
        /// 原始类型的结果特性
        /// </summary>
        /// <param name="acceptQuality">accept的质比</param>
        public RawReturnAttribute(double acceptQuality)
            : base(acceptQuality)
        {
        }

        /// <summary>
        /// 指示是否可以设置结果
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected override bool CanSetResult(ApiResponseContext context)
        {
            return context.ActionDescriptor.Return.DataType.IsRawType;
        }

        /// <summary>
        /// 设置结果值
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public async override Task SetResultAsync(ApiResponseContext context)
        {
            var response = context.HttpContext.ResponseMessage;
            if (response == null)
            {
                return;
            }

            if (this.EnsureSuccessStatusCode && response.IsSuccessStatusCode == false)
            {
                throw new ApiResponseStatusException(response);
            }

            var dataType = context.ActionDescriptor.Return.DataType;
            if (dataType.IsRawHttpResponseMessage == true)
            {
                context.Result = response;
            }
            else if (dataType.IsRawString == true)
            {
                context.Result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            else if (dataType.IsRawByteArray == true)
            {
                context.Result = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            }
            else if (dataType.IsRawStream == true)
            {
                context.Result = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            }
        }
    }
}
