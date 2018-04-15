using System;

namespace WebApiClient.Token
{
    /// <summary>
    /// 提供获取ITokenApi的http客户端实例
    /// </summary>
    public static class TokenClient
    {
        /// <summary>
        /// 授权服务器域名的客户端实例的缓存
        /// </summary>
        private static ConcurrentCache<Uri, ITokenApi> clientCache = new ConcurrentCache<Uri, ITokenApi>();

        /// <summary>
        /// 返回ITokenApi的http客户端实例
        /// </summary>
        /// <param name="tokenEndpoint">授权服务器地址</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static ITokenApi GetClient(Uri tokenEndpoint)
        {
            if (tokenEndpoint == null)
            {
                throw new ArgumentNullException(nameof(tokenEndpoint));
            }

            return clientCache.GetOrAdd(tokenEndpoint, endpoint =>
            {
                var config = new HttpApiConfig { HttpHost = tokenEndpoint };
                return HttpApiClient.Create<ITokenApi>(config);
            });
        }
    }
}
