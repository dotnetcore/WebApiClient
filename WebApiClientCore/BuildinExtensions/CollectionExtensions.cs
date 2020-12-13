using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApiClientCore
{
    /// <summary>
    /// 提供集合扩展
    /// </summary>
    static class CollectionExtensions
    {
        /// <summary>
        /// 格式化集合
        /// </summary>
        /// <param name="keyValues">集合</param>
        /// <param name="format">格式</param>
        /// <returns></returns>
        public static IEnumerable<KeyValue> CollectAs(this IEnumerable<KeyValue> keyValues, CollectionFormat format)
        {
            return format switch
            {
                CollectionFormat.Multi => keyValues,
                CollectionFormat.Csv => keyValues.CollectAs(@","),
                CollectionFormat.Ssv => keyValues.CollectAs(@" "),
                CollectionFormat.Tsv => keyValues.CollectAs(@"\"),
                CollectionFormat.Pipes => keyValues.CollectAs(@"|"),
                _ => throw new NotImplementedException(format.ToString()),
            };
        }

        /// <summary>
        /// 格式化集合
        /// </summary>
        /// <param name="keyValues">集合</param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        private static IEnumerable<KeyValue> CollectAs(this IEnumerable<KeyValue> keyValues, string separator)
        {
            if (keyValues is ICollection<KeyValue> collection && collection.Count < 2)
            {
                return keyValues;
            }

            return keyValues.GroupBy(item => item.Key).Select(item =>
            {
                var value = string.Join(separator, item.Select(i => i.Value));
                return new KeyValue(item.Key, value);
            });
        }


        /// <summary>
        /// 转换为只读列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return source.ToList().AsReadOnly();
        }
    }
}
