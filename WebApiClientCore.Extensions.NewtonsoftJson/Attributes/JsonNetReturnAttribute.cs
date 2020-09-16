using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading.Tasks;
using WebApiClientCore.Extensions.NewtonsoftJson;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示使用Json.Net处理响应内容的结果特性
    /// </summary>
    public class JsonNetReturnAttribute : JsonReturnAttribute
    {
        /// <summary>
        /// 使用Json.Net处理响应内容的结果特性
        /// </summary>
        public JsonNetReturnAttribute()
            : base()
        {
        }

        /// <summary>
        /// 使用Json.Net处理响应内容的结果特性
        /// </summary>
        /// <param name="acceptQuality">accept的质比</param>
        public JsonNetReturnAttribute(double acceptQuality)
            : base(acceptQuality)
        {
        }

        /// <summary>
        /// 设置强类型模型结果值
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override async Task SetResultAsync(ApiResponseContext context)
        {
            var response = context.HttpContext.ResponseMessage;
            if (response == null || response.Content == null)
            {
                return;
            }

            if (context.ApiAction.Return.DataType.IsRawType == false)
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var resultType = context.ApiAction.Return.DataType.Type;

                var name = context.HttpContext.OptionsName;
                var options = context.HttpContext.ServiceProvider.GetService<IOptionsMonitor<JsonNetSerializerOptions>>().Get(name);

                context.Result = JsonConvert.DeserializeObject(json, resultType, options.JsonDeserializeOptions);
            }
        }
    }
}
