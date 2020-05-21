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
        /// 序列化对象为键值对
        /// </summary>
        /// <param name="key">对象名称</param>
        /// <param name="obj">对象实例</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        public virtual KeyValue[] Serialize(string key, object? obj, JsonSerializerOptions? options)
        {
            if (obj == null)
            {
                if (string.IsNullOrEmpty(key))
                {
                    throw new ArgumentNullException(nameof(key));
                }
                else
                {
                    return new[] { new KeyValue(key, null) };
                }
            }

            var json = JsonSerializer.SerializeToUtf8Bytes(obj, obj.GetType(), options);
            var jsonReader = new Utf8JsonReader(json);
            return GetKeyValues(key, jsonReader);
        }

        /// <summary>
        /// 获取键值对
        /// </summary>
        /// <param name="key"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static KeyValue[] GetKeyValues(string key, Utf8JsonReader reader)
        {
            var result = new List<KeyValue>();
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
                            result.Add(new KeyValue(key, null));
                            break;
                        }
                    case JsonTokenType.False:
                    case JsonTokenType.True:
                    case JsonTokenType.String:
                    case JsonTokenType.Number:
                        {
                            var value = Encoding.UTF8.GetString(reader.ValueSpan);
                            var keyValue = new KeyValue(key, value);
                            result.Add(keyValue);
                            break;
                        }
                }
            }
            return result.ToArray();
        }
    }
}
