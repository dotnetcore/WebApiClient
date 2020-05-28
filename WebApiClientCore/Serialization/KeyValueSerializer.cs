using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace WebApiClientCore.Serialization
{
    /// <summary>
    /// 默认的keyValue序列化工具
    /// </summary>
    public class KeyValueSerializer : IKeyValueSerializer
    {
        /// <summary>
        /// 预留的缓冲区大小
        /// </summary>
        private const int sizeHint = 512;

        /// <summary>
        /// 默认的序列化选项
        /// </summary>
        private static readonly KeyValueSerializerOptions defaultOptions = new KeyValueSerializerOptions();

        /// <summary>
        /// 序列化对象为键值对
        /// </summary>
        /// <param name="key">对象名称</param>
        /// <param name="obj">对象实例</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        public IList<KeyValue> Serialize(string key, object? obj, KeyValueSerializerOptions? options)
        {
            if (obj == null)
            {
                var keyValue = new KeyValue(key, null);
                return new List<KeyValue>(1) { keyValue };
            }

            var objType = obj.GetType();
            if (objType == typeof(string))
            {
                var keyValue = new KeyValue(key, obj.ToString());
                return new List<KeyValue>(1) { keyValue };
            }

            var kvOptions = options ?? defaultOptions;
            var jsonOptions = kvOptions.GetJsonSerializerOptions();
            using var bufferWriter = new BufferWriter<byte>(sizeHint);
            using var utf8JsonWriter = new Utf8JsonWriter(bufferWriter, new JsonWriterOptions
            {
                Indented = false,
                SkipValidation = true,
                Encoder = jsonOptions.Encoder
            });

            System.Text.Json.JsonSerializer.Serialize(utf8JsonWriter, obj, objType, jsonOptions);
            var span = bufferWriter.GetWrittenSpan();
            var utf8JsonReader = new Utf8JsonReader(span, new JsonReaderOptions
            {
                MaxDepth = jsonOptions.MaxDepth,
                CommentHandling = jsonOptions.ReadCommentHandling,
                AllowTrailingCommas = jsonOptions.AllowTrailingCommas,
            });

            return kvOptions.KeyNamingStyle == KeyNamingStyle.ShortName
                ? this.GetShortNameKeyValueList(key, ref utf8JsonReader)
                : this.GetFullNameKeyValueList(key, ref utf8JsonReader, kvOptions.IncludeRootKeyName);
        }


        /// <summary>
        /// 获取shortName键值对
        /// </summary>
        /// <param name="key"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected virtual IList<KeyValue> GetShortNameKeyValueList(string key, ref Utf8JsonReader reader)
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

        /// <summary>
        /// 获取fullName键值对
        /// </summary>
        /// <param name="key"></param>
        /// <param name="reader"></param>
        /// <param name="includeRootKeyName"></param>
        /// <returns></returns>
        protected virtual IList<KeyValue> GetFullNameKeyValueList(string key, ref Utf8JsonReader reader, bool includeRootKeyName)
        {
            var list = new List<KeyValue>();
            var root = JsonDocument.ParseValue(ref reader).RootElement;
            ParseJsonElement(key, ref root, list, includeRootKeyName);
            return list;
        }

        /// <summary>
        /// 解析JsonElement
        /// </summary>
        /// <param name="key"></param>
        /// <param name="element"></param>
        /// <param name="list"></param>
        /// <param name="includeRootKeyName"></param>
        private static void ParseJsonElement(string key, ref JsonElement element, IList<KeyValue> list, bool includeRootKeyName)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Null:
                    list.Add(new KeyValue(key, null));
                    break;

                case JsonValueKind.String:
                    list.Add(new KeyValue(key, element.GetString()));
                    break;

                case JsonValueKind.True:
                case JsonValueKind.False:
                case JsonValueKind.Number:
                    list.Add(new KeyValue(key, element.GetRawText()));
                    break;

                case JsonValueKind.Object:
                    foreach (var item in element.EnumerateObject())
                    {
                        var ele = item.Value;
                        var itemKey = includeRootKeyName ? $"{key}.{item.Name}" : item.Name;
                        ParseJsonElement(itemKey, ref ele, list, includeRootKeyName: true);
                    }
                    break;

                case JsonValueKind.Array:
                    var index = 0;
                    foreach (var item in element.EnumerateArray())
                    {
                        var ele = item;
                        var itemKey = $"{key}[{index}]";
                        ParseJsonElement(itemKey, ref ele, list, includeRootKeyName: true);
                        index += 1;
                    }
                    break;
            }
        }

    }
}