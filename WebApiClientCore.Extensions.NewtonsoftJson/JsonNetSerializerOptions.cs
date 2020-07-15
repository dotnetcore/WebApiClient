using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WebApiClientCore.Extensions.NewtonsoftJson
{
    /// <summary>
    /// json.net选项
    /// </summary>
    public class JsonNetSerializerOptions
    {
        /// <summary>
        /// camelCaseResolver
        /// 不能多次创建，否则影响到反射缓存
        /// </summary>
        private static readonly IContractResolver camelCaseResolver = new CamelCasePropertyNamesContractResolver();

        /// <summary>
        /// 序列化设置
        /// </summary>
        public JsonSerializerSettings JsonSerializeOptions { get; set; } = CreateDefaultJsonOptions();

        /// <summary>
        /// 反序列化设置
        /// </summary>
        public JsonSerializerSettings JsonDeserializeOptions { get; set; } = CreateDefaultJsonOptions();

        /// <summary>
        /// 创建默认JsonSerializerSettings
        /// </summary> 
        private static JsonSerializerSettings CreateDefaultJsonOptions()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = camelCaseResolver,
            };
        }
    }
}
