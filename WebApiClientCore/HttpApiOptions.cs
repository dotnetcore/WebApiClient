using System;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Xml;
using WebApiClientCore.Serialization;
using WebApiClientCore.Serialization.JsonConverters;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示HttpApi选项
    /// </summary>
    public class HttpApiOptions
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private JsonSerializerOptions? jsonSerializeOptions;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private JsonSerializerOptions? jsonDeserializeOptions;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private XmlWriterSettings? xmlSerializeOptions;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private XmlReaderSettings? xmlDeserializeOptions;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private KeyValueSerializerOptions? keyValueSerializeOptions;

        /// <summary>
        /// 获取或设置Http服务完整主机域名
        /// 例如http://www.abc.com/或http://www.abc.com/path/
        /// 设置了HttpHost值，HttpHostAttribute将失效
        /// </summary>
        public Uri? HttpHost { get; set; }

        /// <summary>
        /// 获取或设置是否使用的日志功能
        /// </summary>
        public bool UseLogging { get; set; } = true;

        /// <summary>
        /// 获取或设置请求头是否包含默认的UserAgent
        /// </summary>
        public bool UseDefaultUserAgent { get; set; } = true;

        /// <summary>
        /// 获取或设置是否对参数的属性值进行输入有效性验证
        /// </summary>
        public bool UseParameterPropertyValidate { get; set; } = true;

        /// <summary>
        /// 获取或设置是否对返回值的属性值进行输入有效性验证
        /// </summary>
        public bool UseReturnValuePropertyValidate { get; set; } = true;

        /// <summary>
        /// 获取json序列化选项
        /// </summary>
        public JsonSerializerOptions JsonSerializeOptions
        {
            get
            {
                if (this.jsonSerializeOptions == null)
                {
                    this.jsonSerializeOptions = CreateDefaultJsonOptions();
                }
                return this.jsonSerializeOptions;
            }
        }

        /// <summary>
        /// 获取json反序列化选项
        /// </summary>
        public JsonSerializerOptions JsonDeserializeOptions
        {
            get
            {
                if (this.jsonDeserializeOptions == null)
                {
                    this.jsonDeserializeOptions = CreateDefaultJsonOptions();
                    this.jsonDeserializeOptions.Converters.Add(JsonCompatibleConverter.EnumReader);
                    this.jsonDeserializeOptions.Converters.Add(JsonCompatibleConverter.DateTimeReader);
                    this.jsonDeserializeOptions.Converters.Add(JsonCompatibleConverter.DateTimeOffsetReader);
                }
                return this.jsonDeserializeOptions;
            }
        }

        /// <summary>
        /// xml序列化选项
        /// </summary>
        public XmlWriterSettings XmlSerializeOptions
        {
            get
            {
                if (this.xmlSerializeOptions == null)
                {
                    this.xmlSerializeOptions = new XmlWriterSettings();
                }
                return this.xmlSerializeOptions;
            }
        }

        /// <summary>
        /// xml反序列化选项
        /// </summary>
        public XmlReaderSettings XmlDeserializeOptions
        {
            get
            {
                if (this.xmlDeserializeOptions == null)
                {
                    this.xmlDeserializeOptions = new XmlReaderSettings();
                }
                return this.xmlDeserializeOptions;
            }
        }

        /// <summary>
        /// 获取keyValue序列化选项
        /// </summary>
        public KeyValueSerializerOptions KeyValueSerializeOptions
        {
            get
            {
                if (this.keyValueSerializeOptions == null)
                {
                    this.keyValueSerializeOptions = new KeyValueSerializerOptions();
                }
                return this.keyValueSerializeOptions;
            }
        }


        /// <summary>
        /// 创建默认JsonSerializerOptions
        /// </summary> 
        private static JsonSerializerOptions CreateDefaultJsonOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
        }
    }
}