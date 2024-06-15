using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace WebApiClientCore.Serialization.JsonConverters
{
    /// <summary>
    /// 提供一些类型的兼容性的json转换器
    /// </summary>
    public static class JsonCompatibleConverter
    {
        private static JsonStringEnumConverter? stringEnumConverter;

        /// <summary>
        /// 获取Enum类型反序列化兼容的转换器
        /// </summary>
        public static JsonConverter EnumReader
        {
            [RequiresDynamicCode("JsonStringEnumConverter需要动态代码")]
            get
            {
                stringEnumConverter ??= new JsonStringEnumConverter();
                return stringEnumConverter;
            }
        }

        /// <summary>
        /// 获取DateTime类型反序列化兼容的转换器
        /// </summary>
        public static JsonConverter DateTimeReader { get; } = new JsonDateTimeConverter("O");
    }
}