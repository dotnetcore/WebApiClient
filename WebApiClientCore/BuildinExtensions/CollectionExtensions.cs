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
        /// <param name="collection">集合</param>
        /// <param name="format">格式</param>
        /// <returns></returns>
        public static IEnumerable<KeyValue> CollectAs(this IEnumerable<KeyValue> collection, CollectionFormat format)
        {
            if (format == CollectionFormat.Multi)
            {
                return collection;
            }

            switch (format)
            {
                case CollectionFormat.Csv:
                    return collection.CollectAs(@",");

                case CollectionFormat.Ssv:
                    return collection.CollectAs(@" ");

                case CollectionFormat.Tsv:
                    return collection.CollectAs(@"\");

                case CollectionFormat.Pipes:
                    return collection.CollectAs(@"|");

                default:
                    throw new NotImplementedException(format.ToString());
            }
        }

        /// <summary>
        /// 格式化集合
        /// </summary>
        /// <param name="collection">集合</param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        private static IEnumerable<KeyValue> CollectAs(this IEnumerable<KeyValue> collection, string separator)
        {
            return collection.GroupBy(item => item.Key).Select(item =>
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
