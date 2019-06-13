using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.IO;
using System.Threading.Tasks;
using WebApiClient.Contexts;
using WebApiClient.DataAnnotations;
using WebApiClient.Defaults;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 序列化参数值得到的bson作为application/bson请求
    /// 每个Api只能注明于其中的一个参数
    /// </summary>
    public class BsonContentAttribute : HttpContentAttribute, IDateTimeFormatable
    {
        /// <summary>
        /// 获取或设置时期时间格式
        /// </summary>
        public string DateTimeFormat { get; set; }

        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        protected sealed override Task SetHttpContentAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            return base.SetHttpContentAsync(context, parameter);
        }

        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        protected sealed override void SetHttpContent(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var options = context.HttpApiConfig.FormatOptions.CloneChange(this.DateTimeFormat);
            var setting = this.CreateSerializerSettings(options);
            var serializer = JsonSerializer.Create(setting);

            var stream = new MemoryStream();
            var bsonWriter = new BsonWriter(stream);
            serializer.Serialize(bsonWriter, parameter.Value);
            stream.Seek(0L, SeekOrigin.Begin);
            context.RequestMessage.Content = new BsonContent(stream);
        }

        /// <summary>
        /// 创建序列化配置     
        /// </summary>
        /// <param name="options">格式化选项</param>
        /// <returns></returns>
        protected virtual JsonSerializerSettings CreateSerializerSettings(FormatOptions options)
        {
            var setting= options.ToSerializerSettings(FormatScope.BsonFormat);
            setting.Converters.Add(JsonStringConverter.Instance);
            return setting;
        }
    }
}
