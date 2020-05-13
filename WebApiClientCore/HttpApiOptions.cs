using System;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示HttpApi选项
    /// </summary>
    public class HttpApiOptions
    {
        /// <summary>
        /// 获取或设置是否对参数的属性值进行输入有效性验证
        /// </summary>
        public bool UseParameterPropertyValidate { get; set; } = true;

        /// <summary>
        /// 获取或设置是否对返回值的属性值进行输入有效性验证
        /// </summary>
        public bool UseReturnValuePropertyValidate { get; set; } = true;


        /// <summary>
        /// 获取或设置Http服务完整主机域名
        /// 例如http://www.webapiclient.com
        /// 设置了HttpHost值，HttpHostAttribute将失效  
        /// </summary>
        public Uri HttpHost { get; set; }


        /// <summary>
        /// 获取或设置json序列化选项
        /// </summary>
        public JsonSerializerOptions JsonSerializeOptions { get; set; } = CreateDefaultJsonOptions();

        /// <summary>
        /// 获取或设置json反序列化选项
        /// </summary>
        public JsonSerializerOptions JsonDeserializeOptions { get; set; } = CreateDefaultJsonOptions();

        /// <summary>
        /// 获取或设置keyValue序列化选项
        /// </summary>
        public JsonSerializerOptions KeyValueSerializeOptions { get; set; } = CreateDefaultJsonOptions();

        /// <summary>
        /// 创建默认的json序列化选项
        /// </summary>
        /// <returns></returns>
        public static JsonSerializerOptions CreateDefaultJsonOptions()
        {
            var options = new JsonSerializerOptions
            {
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            options.Converters.Add(JsonStringConverter.Instance);
            return options;
        }
    }

    /// <summary>
    /// 表示HttpApi选项
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    public class HttpApiOptions<THttpApi> : HttpApiOptions where THttpApi : class, IHttpApi
    {
    }
}