using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示json内容的强类型模型结果特性
    /// </summary>
    public class JsonResultAttribute : StrongTypeResultAttribute
    {
        /// <summary>
        /// json内容的强类型模型结果特性
        /// </summary>
        public JsonResultAttribute()
            : base(JsonContent.MediaType)
        {
        }

        /// <summary>
        /// 设置强类型模型结果值
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override async Task SetStrongTypeResultAsync(ApiActionContext context)
        {
            var response = context.HttpContext.ResponseMessage;
            var dataType = context.ApiAction.Return.DataType.Type;

            var formatter = context.HttpContext.Services.GetRequiredService<IJsonFormatter>();
            var json = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var options = context.HttpContext.Options.JsonDeserializeOptions;
            context.Result = formatter.Deserialize(json, dataType, options);
        }
    }
}
