using System.Diagnostics;

namespace WebApiClient
{
    /// <summary>
    /// 表示缓存结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("HasValue = {HasValue}")]
    public struct CacheResult<T>
    {
        /// <summary>
        /// 获取缓存的值
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// 获取是否有缓存的值
        /// </summary>
        public bool HasValue { get; }

        /// <summary>
        /// 缓存结果
        /// </summary>
        /// <param name="value">缓存的值</param>
        /// <param name="hasValue">是否有缓存的值</param>
        public CacheResult(T value, bool hasValue)
        {
            this.Value = value;
            this.HasValue = hasValue;
        }
    }
}
