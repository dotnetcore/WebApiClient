using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace WebApiClientCore.Defaults
{
    /// <summary>
    /// 默认的keyValue序列化工具
    /// </summary>
    public class KeyValueFormatter : IKeyValueFormatter
    {
        /// <summary>
        /// 预留的缓冲区大小
        /// </summary>
        private const int sizeHint = 512;
        private const string trueString = "true";
        private const string falseString = "false";

        /// <summary>
        /// 默认的序列化选项
        /// </summary>
        private static readonly JsonSerializerOptions defaultOptions = new JsonSerializerOptions();

        /// <summary>
        /// 序列化对象为键值对
        /// </summary>
        /// <param name="key">对象名称</param>
        /// <param name="obj">对象实例</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        public virtual IList<KeyValue> Serialize(string key, object? obj, JsonSerializerOptions? options)
        {
            if (obj == null)
            {
                var keyValue = new KeyValue(key, null);
                return new List<KeyValue>(1) { keyValue };
            }

            var typeCode = Type.GetTypeCode(obj.GetType());
            if (typeCode == TypeCode.String)
            {
                var keyValue = new KeyValue(key, obj.ToString());
                return new List<KeyValue>(1) { keyValue };
            }

            if (typeCode == TypeCode.Boolean)
            {
                var boolValue = ((bool)obj) ? trueString : falseString;
                var keyValue = new KeyValue(key, boolValue);
                return new List<KeyValue>(1) { keyValue };
            }

            var jsonOptions = options ?? defaultOptions;
            using var bufferWriter = new ByteBufferWriter(sizeHint);
            using var utf8JsonWriter = new Utf8JsonWriter(bufferWriter, new JsonWriterOptions
            {
                Indented = false,
                SkipValidation = true,
                Encoder = jsonOptions.Encoder
            });

            JsonSerializer.Serialize(utf8JsonWriter, obj, obj.GetType(), options);
            var span = bufferWriter.GetWrittenSpan();
            var utf8JsonReader = new Utf8JsonReader(span, new JsonReaderOptions
            {
                MaxDepth = jsonOptions.MaxDepth,
                CommentHandling = jsonOptions.ReadCommentHandling,
                AllowTrailingCommas = jsonOptions.AllowTrailingCommas,
            });
            return GetKeyValueList(key, ref utf8JsonReader);
        }

        /// <summary>
        /// 获取键值对
        /// </summary>
        /// <param name="key"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static IList<KeyValue> GetKeyValueList(string key, ref Utf8JsonReader reader)
        {
            var list = new List<KeyValue>();
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName:
                        {
                            key = reader.GetString();
                            break;
                        }

                    case JsonTokenType.Null:
                        {
                            list.Add(new KeyValue(key, null));
                            break;
                        }
                    case JsonTokenType.False:
                    case JsonTokenType.True:
                    case JsonTokenType.String:
                    case JsonTokenType.Number:
                        {
                            var value = Encoding.UTF8.GetString(reader.ValueSpan);
                            var keyValue = new KeyValue(key, value);
                            list.Add(keyValue);
                            break;
                        }
                }
            }
            return list;
        }
    }
}
