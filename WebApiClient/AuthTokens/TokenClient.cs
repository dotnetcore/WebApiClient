using System;

namespace WebApiClient.AuthTokens
{
    /// <summary>
    /// 提供获取ITokenApi的http客户端实例
    /// </summary>
    public static class TokenClient
    {
        /// <summary>
        /// 授权服务器域名的客户端实例的缓存
        /// </summary>
        private static ConcurrentCache<Uri, ITokenToken> cache = new ConcurrentCache<Uri, ITokenToken>();

        /// <summary>
        /// 返回ITokenClient的客户端实例
        /// </summary>
        /// <param name="tokenEndpoint">授权服务器地址</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static ITokenToken Get(string tokenEndpoint)
        {
            if (string.IsNullOrEmpty(tokenEndpoint))
            {
                throw new ArgumentNullException(nameof(tokenEndpoint));
            }

            var endPoint = new Uri(tokenEndpoint);
            return TokenClient.Get(endPoint);
        }

        /// <summary>
        /// 返回ITokenClient的客户端实例
        /// </summary>
        /// <param name="tokenEndpoint">授权服务器地址</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static ITokenToken Get(Uri tokenEndpoint)
        {
            if (tokenEndpoint == null)
            {
                throw new ArgumentNullException(nameof(tokenEndpoint));
            }

            return cache.GetOrAdd(tokenEndpoint, endpoint =>
            {
                var config = new HttpApiConfig { HttpHost = endpoint };
                return HttpApiClient.Create<ITokenToken>(config);
            });
        }
    }
}
