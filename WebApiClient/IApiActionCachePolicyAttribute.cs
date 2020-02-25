using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// Define ApiAction caching strategy
    /// </summary>
    public interface IApiActionCachePolicyAttribute : IApiActionCacheAttribute
    {
        /// <summary>
        /// Return read cache strategy       
        /// </summary>
        /// <param name="context">Context</param>
        /// <returns></returns>
        CachePolicy GetReadPolicy(ApiActionContext context);

        /// <summary>
        /// Return write cache strategy
        /// </summary>
        /// <param name="context">Context</param>
        /// <returns></returns>
        CachePolicy GetWritePolicy(ApiActionContext context);
    }

    /// <summary>
    /// Caching strategy
    /// </summary>
    public enum CachePolicy
    {
        /// <summary>
        /// Try to use cache
        /// </summary>
        Include,

        /// <summary>
        /// Ignore and skip cache
        /// </summary>
        Ignore
    }
}
