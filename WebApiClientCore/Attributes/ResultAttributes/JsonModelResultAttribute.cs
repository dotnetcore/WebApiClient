using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示json内容的强类型模型结果特性
    /// </summary>
    public class JsonModelResultAttribute : ApiResultAttribute
    {
        /// <summary>
        /// json内容的强类型模型结果特性
        /// </summary>
        public JsonModelResultAttribute()
            : base(new MediaTypeWithQualityHeaderValue(JsonContent.MediaType))
        {
        }

        /// <summary>
        /// json内容的强类型模型结果特性
        /// </summary>
        /// <param name="acceptQuality">accept的质比</param>
        public JsonModelResultAttribute(double acceptQuality)
            : base(new MediaTypeWithQualityHeaderValue(JsonContent.MediaType, acceptQuality))
        {
        }

        /// <summary>
        /// 设置强类型模型结果值
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override async Task SetResultAsync(ApiResponseContext context)
        {
            if (context.ApiAction.Return.DataType.IsModelType == true)
            {
                var resultType = context.ApiAction.Return.DataType.Type;
                context.Result = await context.JsonDeserializeAsync(resultType).ConfigureAwait(false);
            }
        }
    }
}
