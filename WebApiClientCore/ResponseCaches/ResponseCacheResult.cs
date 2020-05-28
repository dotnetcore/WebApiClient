using System.Diagnostics;

namespace WebApiClientCore.ResponseCaches
{
    /// <summary>
    /// 表示响应缓存结果
    /// </summary>
    [DebuggerDisplay("HasValue = {HasValue}")]
    public struct ResponseCacheResult
    {
        /// <summary>
        /// 表示无Value的缓存结果 
        /// </summary>
        public static readonly ResponseCacheResult NoValue = new ResponseCacheResult(null, false);

        /// <summary>
        /// 获取缓存的值
        /// </summary>
        public ResponseCacheEntry? Value { get; }

        /// <summary>
        /// 获取是否有缓存的值
        /// </summary>
        public bool HasValue { get; }

        /// <summary>
        /// 响应缓存结果
        /// </summary>
        /// <param name="value">缓存的值</param>
        /// <param name="hasValue">是否有缓存的值</param>
        public ResponseCacheResult(ResponseCacheEntry? value, bool hasValue)
        {
            this.Value = value;
            this.HasValue = hasValue;
        }
    }
}
