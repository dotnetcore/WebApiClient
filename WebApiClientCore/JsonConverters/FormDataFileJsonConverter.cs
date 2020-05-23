using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebApiClientCore.Exceptions;
using WebApiClientCore.Parameters;

namespace WebApiClientCore.JsonConverters
{
    /// <summary>
    /// FormDataFile序列化转换器
    /// </summary>
    class FormDataFileJsonConverter : JsonConverter<FormDataFile>
    {
        /// <summary>
        /// 获取默认实例
        /// </summary>
        public static FormDataFileJsonConverter Default { get; } = new FormDataFileJsonConverter();

        /// <summary>
        /// 读取json
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override FormDataFile Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new ApiParameterSerializeException(typeToConvert);
        }

        /// <summary>
        /// 写json
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, FormDataFile value, JsonSerializerOptions options)
        {
            throw new ApiParameterSerializeException(typeof(FormDataFile));
        }
    }
}
