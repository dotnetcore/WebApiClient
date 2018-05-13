using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace WebApiClient.Defaults
{
    /// <summary>
    /// 表示KeyValuePair转换器
    /// </summary>
    class KeyValuePairConverter : JsonConverter
    {
        /// <summary>
        /// KeyValuePair泛型
        /// </summary>
        private static readonly Type keyValuePairType = typeof(KeyValuePair<,>);

        /// <summary>
        /// 返回是否支持转换目标类型
        /// </summary>
        /// <param name="objectType">目标类型</param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType.Detail().IsGenericType && objectType.GetGenericTypeDefinition() == keyValuePairType;
        }

        /// <summary>
        /// 从json解析得到对象
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 解析为json
        /// 实际解析为KeyValuePair类型
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var reader = KeyValuePairReader.GetReader(value.GetType());
            var key = reader.GetKey(value);
            var val = reader.GetValue(value);

            writer.WritePropertyName(key.ToString());
            writer.WriteValue(val);
        }

        /// <summary>
        /// 表示KeyValuePair读取器
        /// </summary>
        private class KeyValuePairReader
        {
            /// <summary>
            /// key的getter
            /// </summary>
            private readonly PropertyGetter keyGetter;

            /// <summary>
            /// value的getter
            /// </summary>
            private readonly PropertyGetter valueGetter;

            /// <summary>
            /// KeyValuePair读取器
            /// </summary>
            /// <param name="keyValuePairType">KeyValuePair的类型</param>
            private KeyValuePairReader(Type keyValuePairType)
            {
                this.keyGetter = new PropertyGetter(keyValuePairType, "Key");
                this.valueGetter = new PropertyGetter(keyValuePairType, "Value");
            }

            /// <summary>
            /// 返回实例的Key的值
            /// </summary>
            /// <param name="instance">实例</param>
            /// <returns></returns>
            public object GetKey(object instance)
            {
                return this.keyGetter.Invoke(instance);
            }

            /// <summary>
            /// 返回实例的Value的值
            /// </summary>
            /// <param name="instance">实例</param>
            /// <returns></returns>
            public object GetValue(object instance)
            {
                return this.valueGetter.Invoke(instance);
            }

            /// <summary>
            /// 类型的KeyValuePairReader缓存
            /// </summary>
            private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, KeyValuePairReader> readerCache = new System.Collections.Concurrent.ConcurrentDictionary<Type, KeyValuePairReader>();

            /// <summary>
            /// 从类型获取KeyValuePairReader
            /// </summary>
            /// <param name="type">类型</param>
            /// <returns></returns>
            public static KeyValuePairReader GetReader(Type type)
            {
                return readerCache.GetOrAdd(type, t => new KeyValuePairReader(t));
            }
        }
    }
}