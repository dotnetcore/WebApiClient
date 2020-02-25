using System;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// Defines the interface of the Api response result provider
    /// </summary>
    public interface IResponseCacheProvider
    {
        /// <summary>
        /// Get the friendly name of the provider
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get response entity from cache
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        Task<ResponseCacheResult> GetAsync(string key);

        /// <summary>
        /// Set response entity to cache
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="entry">Cache entity</param>
        /// <param name="expiration">Effective time</param>
        /// <returns></returns>
        Task SetAsync(string key, ResponseCacheEntry entry, TimeSpan expiration);
    }
}
