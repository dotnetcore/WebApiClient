using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using WebApiClientCore.Serialization;

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
        public Uri? HttpHost { get; set; }


        /// <summary>
        /// 获取json序列化选项
        /// </summary>
        public JsonSerializerOptions JsonSerializeOptions { get; } = new JsonSerializerOptions();

        /// <summary>
        /// 获取json反序列化选项
        /// </summary>
        public JsonSerializerOptions JsonDeserializeOptions { get; } = new JsonSerializerOptions();

        /// <summary>
        /// 获取keyValue序列化选项
        /// </summary>
        public KeyValueSerializerOptions KeyValueSerializeOptions { get; } = new KeyValueSerializerOptions();

        /// <summary>
        /// HttpApi选项
        /// </summary>
        public HttpApiOptions()
        {
            ConfigureDefault(this.JsonSerializeOptions);
            ConfigureDefault(this.JsonDeserializeOptions);
            ConfigureDefault(this.KeyValueSerializeOptions.GetJsonSerializerOptions());
        }

        /// <summary>
        /// 配置默认值
        /// </summary>
        /// <param name="options"></param> 
        private static void ConfigureDefault(JsonSerializerOptions options)
        {
            options.PropertyNameCaseInsensitive = true;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        }
    }

    /// <summary>
    /// 表示HttpApi选项
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    public class HttpApiOptions<THttpApi> : HttpApiOptions
    {
    }
}