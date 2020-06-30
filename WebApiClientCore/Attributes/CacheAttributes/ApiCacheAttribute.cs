using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WebApiClientCore.ResponseCaches;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示使用缓存的特性
    /// </summary>
    [DebuggerDisplay("Expiration = {Expiration}")]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class ApiCacheAttribute : Attribute, IApiCacheAttribute
    {
        /// <summary>
        /// 获取缓存的时间戳
        /// </summary>
        public TimeSpan Expiration { get; }

        /// <summary>
        /// 使用缓存的特性
        /// </summary>
        /// <param name="expiration">缓存毫秒数</param>
        public ApiCacheAttribute(double expiration)
            : this(TimeSpan.FromMilliseconds(expiration))
        {
        }

        /// <summary>
        /// 使用缓存的特性
        /// </summary>
        /// <param name="expiration"></param>
        protected ApiCacheAttribute(TimeSpan expiration)
        {
            this.Expiration = expiration;
        }

        /// <summary>
        /// 返回读取缓存的策略
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public virtual CachePolicy GetReadPolicy(ApiRequestContext context)
        {
            return CachePolicy.Include;
        }

        /// <summary>
        /// 返回写入缓存的策略
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public virtual CachePolicy GetWritePolicy(ApiRequestContext context)
        {
            return CachePolicy.Include;
        }


        /// <summary>
        /// 获取缓存提供者
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual IResponseCacheProvider? GetCacheProvider(ApiRequestContext context)
        {
            return context.HttpContext.ServiceProvider.GetService<IResponseCacheProvider>();
        }

        /// <summary>
        /// 返回缓存的键
        /// 该键用于读取或写入缓存到缓存提供者
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract Task<string> GetCacheKeyAsync(ApiRequestContext context);
    }
}
