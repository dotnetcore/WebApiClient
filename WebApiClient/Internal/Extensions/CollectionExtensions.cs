using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApiClient
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
        public static IEnumerable<KeyValuePair<string, string>> FormateAs(this IEnumerable<KeyValuePair<string, string>> collection, CollectionFormat format)
        {
            if (format == CollectionFormat.Multi)
            {
                return collection;
            }

            switch (format)
            {
                case CollectionFormat.Csv:
                    return collection.FormateAs(@",");

                case CollectionFormat.Ssv:
                    return collection.FormateAs(@" ");

                case CollectionFormat.Tsv:
                    return collection.FormateAs(@"\");

                case CollectionFormat.Pipes:
                    return collection.FormateAs(@"|");

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 格式化集合
        /// </summary>
        /// <param name="collection">集合</param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        private static IEnumerable<KeyValuePair<string, string>> FormateAs(this IEnumerable<KeyValuePair<string, string>> collection, string separator)
        {
            return collection.GroupBy(item => item.Key).Select(item =>
            {
                var value = string.Join(separator, item.Select(i => i.Value));
                return new KeyValuePair<string, string>(item.Key, value);
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
