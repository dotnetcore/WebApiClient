using System.Diagnostics;

namespace WebApiClient
{
    /// <summary>
    /// Respond to cached results
    /// </summary>
    [DebuggerDisplay("HasValue = {HasValue}")]
    public struct ResponseCacheResult
    {
        /// <summary>
        /// Represents cached results without Value 
        /// </summary>
        public static readonly ResponseCacheResult NoValue = new ResponseCacheResult(null, false);

        /// <summary>
        /// Get the cached value
        /// </summary>
        public ResponseCacheEntry Value { get; }

        /// <summary>
        /// Get whether there is a cached value
        /// </summary>
        public bool HasValue { get; }

        /// <summary>
        /// Response caching results
        /// </summary>
        /// <param name="value">Cached value</param>
        /// <param name="hasValue">Is there a cached value</param>
        public ResponseCacheResult(ResponseCacheEntry value, bool hasValue)
        {
            this.Value = value;
            this.HasValue = hasValue;
        }
    }
}
