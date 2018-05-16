using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace WebApiClient
{
    /// <summary>
    /// 表示线程安全的内存缓存
    /// </summary>
    /// <typeparam name="TKey">键</typeparam>
    /// <typeparam name="TValue">值</typeparam>
    class ConcurrentCache<TKey, TValue>
    {
        /// <summary>
        /// 线程安全字典
        /// </summary>
        private readonly ConcurrentDictionary<TKey, Lazy<TValue>> dictionary;

        /// <summary>
        /// 线程安全的内存缓存
        /// </summary>
        public ConcurrentCache()
        {
            this.dictionary = new ConcurrentDictionary<TKey, Lazy<TValue>>();
        }

        /// <summary>
        /// 线程安全的内存缓存
        /// </summary>
        /// <param name="comparer">键的比较器</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ConcurrentCache(IEqualityComparer<TKey> comparer)
        {
            this.dictionary = new ConcurrentDictionary<TKey, Lazy<TValue>>(comparer);
        }

        /// <summary>
        /// 获取或添加缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="valueFactory">生成缓存内容的委托</param>
        /// <returns></returns>
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            return this.dictionary
                .GetOrAdd(key, k => new Lazy<TValue>(() => valueFactory(k), LazyThreadSafetyMode.ExecutionAndPublication))
                .Value;
        }
    }
}
