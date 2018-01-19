using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace WebApiClient.Defaults.KeyValueFormats.Converters
{
    /// <summary>
    /// 表示KeyValuePair类型转换器
    /// </summary>
    public class KeyValuePairConverter : ConverterBase
    {
        /// <summary>
        /// KeyValuePair泛型
        /// </summary>
        private static readonly Type keyValuePairType = typeof(KeyValuePair<,>);

        /// <summary>
        /// 执行转换
        /// </summary>
        /// <param name="context">转换上下文</param>
        /// <returns></returns>
        public override IEnumerable<KeyValuePair<string, string>> Invoke(ConvertContext context)
        {
            var type = context.DataType;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == keyValuePairType)
            {
                var reader = KeyValuePairReader.GetReader(type);
                var key = reader.GetKey(context.Data).ToString();
                var value = reader.GetValue(context.Data);

                var ctx = new ConvertContext(key, value, context.Depths, context.Options);
                return ctx.ToKeyValuePairs();
            }

            return this.Next.Invoke(context);
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
            private static readonly ConcurrentDictionary<Type, KeyValuePairReader> readerCache = new ConcurrentDictionary<Type, KeyValuePairReader>();

            /// <summary>
            /// 从类型获取KeyValuePairReader
            /// </summary>
            /// <param name="keyValuePairType">KeyValuePair的类型</param>
            /// <returns></returns>
            public static KeyValuePairReader GetReader(Type keyValuePairType)
            {
                return readerCache.GetOrAdd(keyValuePairType, type => new KeyValuePairReader(type));
            }
        }
    }
}
