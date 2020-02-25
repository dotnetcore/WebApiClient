using System;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// Defines the behavior of the ApiAction cache modifier
    /// </summary>
    public interface IApiActionCacheAttribute
    {
        /// <summary>
        /// Get cached timestamp
        /// </summary>
        TimeSpan Expiration { get; }

        /// <summary>
        /// Returns the cache key corresponding to the request
        /// This key is used to read or write the cache to the cache provider
        /// </summary>
        /// <param name="context">Context</param>
        /// <returns></returns>
        Task<string> GetCacheKeyAsync(ApiActionContext context);
    }
}
