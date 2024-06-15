using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApiClientCore.Serialization.JsonConverters
{
    /// <summary>
    /// 表示JsonString的类型转换器
    /// </summary>
    public class JsonStringTypeConverter : JsonConverterFactory
    {
        /// <summary>
        /// 获取默认实例
        /// </summary>
        public static JsonStringTypeConverter Default { get; } = new JsonStringTypeConverter();

        /// <summary>
        /// 返回是否可以转换
        /// </summary>
        /// <param name="typeToConvert"></param>
        /// <returns></returns>
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsInheritFrom<IJsonString>();
        }

        /// <summary>
        /// 创建转换器
        /// </summary>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
#if NET5_0_OR_GREATER
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Converter<>))]
        [UnconditionalSuppressMessage("ReflectionAnalysis", "IL3050", Justification = "类型已使用DynamicDependency来阻止被裁剪")]
#endif
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return typeof(Converter<>).MakeGenericType(typeToConvert).CreateInstance<JsonConverter>();
        }

        /// <summary>
        /// 转换器
        /// </summary>
        /// <typeparam name="TJsonString"></typeparam>
        private class Converter<TJsonString> : JsonConverter<TJsonString> where TJsonString : IJsonString
        {
            /// <summary>
            /// 将json文本反序列化JsonString的Value的类型
            /// 并构建JsonString类型并返回
            /// </summary>
            /// <param name="reader"></param>
            /// <param name="typeToConvert"></param>
            /// <param name="options"></param>
            /// <returns></returns> 
#if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
            [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "<Pending>")]
            [UnconditionalSuppressMessage("AOT", "IL2067", Justification = "<Pending>")]
#endif
            public override TJsonString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var json = reader.GetString();
                var valueType = typeToConvert.GenericTypeArguments.First();
                var value = string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize(json, valueType, options);
                return typeToConvert.CreateInstance<TJsonString>(value);
            }

            /// <summary>
            /// 将JsonString的value序列化文本，并作为json的某字段值
            /// </summary>
            /// <param name="writer"></param>
            /// <param name="value"></param>
            /// <param name="options"></param>
#if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
            [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "<Pending>")]
#endif
            public override void Write(Utf8JsonWriter writer, TJsonString value, JsonSerializerOptions options)
            {
                if (value == null || value.Value == null)
                {
                    writer.WriteStringValue(default(string));
                }
                else
                {
                    var json = JsonSerializer.Serialize(value.Value, value.ValueType, options);
                    writer.WriteStringValue(json);
                }
            }
        }
    }
}
