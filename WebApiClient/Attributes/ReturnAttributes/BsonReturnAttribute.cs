using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApiClient.Contexts;
using WebApiClient.DataAnnotations;
using WebApiClient.Defaults;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示使用Bson反序列化回复内容作为返回值
    /// </summary>
    public class BsonReturnAttribute : ApiReturnAttribute
    {
        /// <summary>
        /// 配置请求头的accept
        /// </summary>
        /// <param name="accept">请求头的accept</param>
        /// <returns></returns>
        protected override void ConfigureAccept(HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> accept)
        {
            accept.Add(new MediaTypeWithQualityHeaderValue("application/bson"));
        }

        /// <summary>
        /// 获取异步结果
        /// </summary>
        /// <param name="context">上下文</param>
        /// <exception cref="HttpStatusFailureException"></exception>
        /// <returns></returns>
        protected override async Task<object> GetTaskResult(ApiActionContext context)
        {
            var options = context.HttpApiConfig.FormatOptions;
            var setting = this.CreateSerializerSettings(options);
            var serializer = JsonSerializer.Create(setting);

            var stream = await context.ResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var bsonReader = new BsonReader(stream);
            var dataType = context.ApiActionDescriptor.Return.DataType.Type;
            return serializer.Deserialize(bsonReader, dataType);
        }

        /// <summary>
        /// 创建序列化配置     
        /// </summary>
        /// <param name="options">格式化选项</param>
        /// <returns></returns>
        protected virtual JsonSerializerSettings CreateSerializerSettings(FormatOptions options)
        {
            var useCamelCase = options?.UseCamelCase == true;
            return new JsonSerializerSettings
            {
                DateFormatString = options?.DateTimeFormat,
                ContractResolver = AnnotationsContractResolver.GetResolver(FormatScope.BsonFormat, useCamelCase)
            };
        }
    }
}
