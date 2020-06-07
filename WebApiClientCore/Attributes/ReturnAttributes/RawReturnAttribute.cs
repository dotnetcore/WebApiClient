using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示原始类型的结果特性
    /// 支持结果类型为string、byte[]、Stream和HttpResponseMessage
    /// </summary>
    public sealed class RawReturnAttribute : ApiReturnAttribute
    {
        /// <summary>
        /// accept quality
        /// </summary>
        private readonly int orderIndex;

        /// <summary>
        /// 获取执行排序索引
        /// </summary>
        public override int OrderIndex => this.orderIndex;

        /// <summary>
        /// 原始类型的结果特性
        /// </summary>
        public RawReturnAttribute()
            : this(1d)
        {
        }

        /// <summary>
        /// 原始类型的结果特性
        /// </summary>
        /// <param name="acceptQuality">accept的质比</param>
        public RawReturnAttribute(double acceptQuality)
            : base(null)
        {
            this.orderIndex = (int)((1d - acceptQuality) * int.MaxValue);
            this.EnsureMatchAcceptContentType = false;
        }

        /// <summary>
        /// 指示响应的ContentType与Accept-ContentType是否匹配
        /// 返回false则调用下一个ApiReturnAttribute来处理响应结果
        /// </summary>
        /// <param name="responseContentType"></param>
        /// <returns></returns>
        protected override bool IsMatchAcceptContentType(MediaTypeHeaderValue? responseContentType)
        {
            return true;
        }

        /// <summary>
        /// 设置结果值
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override async Task SetResultAsync(ApiResponseContext context)
        {
            var dataType = context.ApiAction.Return.DataType;
            var response = context.HttpContext.ResponseMessage;

            if (dataType.IsRawType == false || response == null)
            {
                return;
            }

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
