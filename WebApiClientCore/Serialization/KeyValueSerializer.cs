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
            var kvOptions = options ?? defaultOptions;
            if (obj == null)
            {
                if (kvOptions.IgnoreNullValues == true)
                {
                    return new List<KeyValue>();
                }
                var keyValue = new KeyValue(key, null);
                return new List<KeyValue>(1) { keyValue };
            }

            var objType = obj.GetType();
            if (objType == typeof(string))
            {
                var keyValue = new KeyValue(key, obj.ToString());
                return new List<KeyValue>(1) { keyValue };
            }

            var jsonOptions = kvOptions.GetJsonSerializerOptions();
            using var bufferWriter = new BufferWriter<byte>();
            using var utf8JsonWriter = new Utf8JsonWriter(bufferWriter, new JsonWriterOptions
            {
                Indented = false,
                SkipValidation = true,
                Encoder = jsonOptions.Encoder
            });

            System.Text.Json.JsonSerializer.Serialize(utf8JsonWriter, obj, objType, jsonOptions);
            var utf8JsonReader = new Utf8JsonReader(bufferWriter.GetWrittenSpan(), new JsonReaderOptions
            {
                MaxDepth = jsonOptions.MaxDepth,
                CommentHandling = jsonOptions.ReadCommentHandling,
                AllowTrailingCommas = jsonOptions.AllowTrailingCommas,
            });

            return this.GetKeyValueList(key, ref utf8JsonReader, kvOptions);
        }

        /// <summary>
        /// 获取键值对
        /// </summary>
        /// <param name="key">根键名</param>
        /// <param name="reader">utf8JsonReader</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        protected virtual IList<KeyValue> GetKeyValueList(string key, ref Utf8JsonReader reader, KeyNamingOptions options)
        {
            if (options.KeyNamingStyle == KeyNamingStyle.ShortName)
            {
                return GetShortNameKeyValueList(key, ref reader);
            }
            else
            {
                return GetFullNameKeyValueList(key, ref reader, options);
            }
        }

        /// <summary>
        /// 获取shortName键值对
        /// </summary>
        /// <param name="key"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static IList<KeyValue> GetShortNameKeyValueList(string key, ref Utf8JsonReader reader)
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
        /// <param name="options"></param>
        /// <returns></returns>
        private static IList<KeyValue> GetFullNameKeyValueList(string key, ref Utf8JsonReader reader, KeyNamingOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;
            var list = new List<KeyValue>();
            var withRoot = options.KeyNamingStyle == KeyNamingStyle.FullNameWithRoot;
            ParseJsonElement(key, ref root, list, options, withRoot);
            return list;
        }

        /// <summary>
        /// 解析JsonElement
        /// </summary>
        /// <param name="key"></param>
        /// <param name="element"></param>
        /// <param name="list"></param>
        /// <param name="options"></param>
        /// <param name="withRoot"></param>
        private static void ParseJsonElement(string key, ref JsonElement element, IList<KeyValue> list, KeyNamingOptions options, bool withRoot = true)
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
                        var itemKey = withRoot
                            ? $"{key}{options.KeyDelimiter}{item.Name}"
                            : item.Name;

                        ParseJsonElement(itemKey, ref ele, list, options);
                    }
                    break;

                case JsonValueKind.Array:
                    var index = 0;
                    foreach (var item in element.EnumerateArray())
                    {
                        var ele = item;
                        var itemKey = $"{key}{options.KeyArrayIndex(index)}";

                        ParseJsonElement(itemKey, ref ele, list, options);
                        index += 1;
                    }
                    break;
            }
        }

    }
}