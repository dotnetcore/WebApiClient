using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApiClientCore.Serialization.JsonConverters
{
    /// <summary>
    /// 提供一些类型的兼容性的json转换器
    /// </summary>
    public static class JsonCompatibleConverter
    {
        /// <summary>
        /// 获取Enum类型反序列化兼容的转换器
        /// </summary>
        public static JsonConverter EnumReader { get; } = new JsonEnumReader();

        /// <summary>
        /// 获取DateTime类型反序列化兼容的转换器
        /// </summary>
        public static JsonConverter DateTimeReader { get; } = new JsonDateTimeReader();

        /// <summary>
        /// 获取 DateTimeOffset类型反序列化兼容的转换器
        /// </summary>
        public static JsonConverter DateTimeOffsetReader { get; } = new JsonDateTimeOffsetReader();


        /// <summary>
        /// 表示DateTime的本地格式化Json转换器
        /// </summary>
        private class JsonDateTimeReader : JsonConverter<DateTime>
        {
            /// <summary>
            /// 读取时间
            /// 统一转换为本地时间
            /// </summary>
            /// <param name="reader"></param>
            /// <param name="typeToConvert"></param>
            /// <param name="options"></param>
            /// <returns></returns>
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    if (DateTime.TryParse(reader.GetString(), out var dateTime))
                    {
                        return dateTime;
                    }
                }
                return reader.GetDateTime();
            }

            /// <summary>
            /// 写入时间 
            /// </summary>
            /// <param name="writer"></param>
            /// <param name="value"></param>
            /// <param name="options"></param>
            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                throw new NotSupportedException();
            }
        }


        /// <summary>
        /// 表示DateTimeOffset的本地格式化Json转换器
        /// </summary>
        private class JsonDateTimeOffsetReader : JsonConverter<DateTimeOffset>
        {
            /// <summary>
            /// 读取时间
            /// 统一转换为本地时间
            /// </summary>
            /// <param name="reader"></param>
            /// <param name="typeToConvert"></param>
            /// <param name="options"></param>
            /// <returns></returns>
            public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    if (DateTimeOffset.TryParse(reader.GetString(), out var dateTime))
                    {
                        return dateTime;
                    }
                }
                return reader.GetDateTimeOffset();
            }

            /// <summary>
            /// 写入时间 
            /// </summary>
            /// <param name="writer"></param>
            /// <param name="value"></param>
            /// <param name="options"></param>
            public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Enum读取器
        /// </summary>
        private class JsonEnumReader : JsonConverterFactory
        {
            /// <summary>
            /// 是否支持转换
            /// </summary>
            /// <param name="typeToConvert"></param>
            /// <returns></returns>
            public override bool CanConvert(Type typeToConvert)
            {
                return typeToConvert.IsEnum;
            }

            /// <summary>
            /// 创建转换器
            /// </summary>
            /// <param name="typeToConvert"></param>
            /// <param name="options"></param>
            /// <returns></returns>
            public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
            {
                var converterType = typeof(JsonEnumConverter<>).MakeGenericType(typeToConvert);
                return converterType.CreateInstance<JsonConverter>();
            }

            /// <summary>
            /// 枚举转换器
            /// </summary>
            /// <typeparam name="T"></typeparam>
            private class JsonEnumConverter<T> : JsonConverter<T> where T : struct, Enum
            {
                /// <summary>
                /// 类型码
                /// </summary>
                private static readonly TypeCode typeCode = Type.GetTypeCode(typeof(T));

                /// <summary>
                /// 是否支持转换
                /// </summary>
                /// <param name="type"></param>
                /// <returns></returns>
                public override bool CanConvert(Type type)
                {
                    return type.IsEnum;
                }

                /// <summary>
                /// 读取枚举
                /// </summary>
                /// <param name="reader"></param>
                /// <param name="typeToConvert"></param>
                /// <param name="options"></param>
                /// <returns></returns>
                public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
                {
                    var token = reader.TokenType;
                    if (token == JsonTokenType.String)
                    {
                        var enumString = reader.GetString();
                        return Enum.Parse<T>(enumString, ignoreCase: true);
                    }

                    if (token == JsonTokenType.Number)
                    {
                        switch (typeCode)
                        {
                            case TypeCode.Int32:
                                if (reader.TryGetInt32(out int int32))
                                {
                                    return Unsafe.As<int, T>(ref int32);
                                }
                                break;
                            case TypeCode.UInt32:
                                if (reader.TryGetUInt32(out uint uint32))
                                {
                                    return Unsafe.As<uint, T>(ref uint32);
                                }
                                break;

                            case TypeCode.Byte:
                                if (reader.TryGetByte(out byte byteValue))
                                {
                                    return Unsafe.As<byte, T>(ref byteValue);
                                }
                                break;
                            case TypeCode.SByte:
                                if (reader.TryGetSByte(out sbyte sbyteValue))
                                {
                                    return Unsafe.As<sbyte, T>(ref sbyteValue);
                                }
                                break;

                            case TypeCode.Int64:
                                if (reader.TryGetInt64(out long int64))
                                {
                                    return Unsafe.As<long, T>(ref int64);
                                }
                                break;
                            case TypeCode.UInt64:
                                if (reader.TryGetUInt64(out ulong uint64))
                                {
                                    return Unsafe.As<ulong, T>(ref uint64);
                                }
                                break;

                            case TypeCode.Int16:
                                if (reader.TryGetInt16(out short int16))
                                {
                                    return Unsafe.As<short, T>(ref int16);
                                }
                                break;
                            case TypeCode.UInt16:
                                if (reader.TryGetUInt16(out ushort uint16))
                                {
                                    return Unsafe.As<ushort, T>(ref uint16);
                                }
                                break;
                        }
                    }

                    var message = Resx.unsupported_ConvertType.Format(reader.TokenType, typeToConvert);
                    throw new NotSupportedException(message);
                }

                /// <summary>
                /// 写入
                /// </summary>
                /// <param name="writer"></param>
                /// <param name="value"></param>
                /// <param name="options"></param>
                public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
                {
                    throw new NotSupportedException();
                }
            }
        }
    }
}