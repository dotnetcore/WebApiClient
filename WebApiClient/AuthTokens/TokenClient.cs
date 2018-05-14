using System;
using WebApiClient.Defaults;

namespace WebApiClient.AuthTokens
{
    /// <summary>
    /// 提供获取ITokenClient的客户端实例
    /// </summary>
    public static class TokenClient
    {
        /// <summary>
        /// 授权服务器域名的客户端实例的缓存
        /// </summary>
        private static readonly ConcurrentCache<Uri, ITokenClient> cache = new ConcurrentCache<Uri, ITokenClient>();

        /// <summary>
        /// 返回ITokenClient的客户端实例
        /// </summary>
        /// <param name="tokenEndpoint">授权服务器地址</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static ITokenClient Get(string tokenEndpoint)
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
        public static ITokenClient Get(Uri tokenEndpoint)
        {
            if (tokenEndpoint == null)
            {
                throw new ArgumentNullException(nameof(tokenEndpoint));
            }

            return cache.GetOrAdd(tokenEndpoint, endpoint =>
            {
                var config = new HttpApiConfig { HttpHost = endpoint };
                var apiInterceptor = new NoneDisposeApiInterceptor(config);
                return HttpApiClient.Create(typeof(ITokenClient), apiInterceptor) as ITokenClient;
            });
        }


        /// <summary>
        /// 表示不需要Dispose的拦截器
        /// </summary>
        private class NoneDisposeApiInterceptor : ApiInterceptor
        {
            /// <summary>
            /// http接口调用的拦截器
            /// </summary>
            /// <param name="httpApiConfig">httpApi配置</param>
            /// <exception cref="ArgumentNullException"></exception>
            public NoneDisposeApiInterceptor(HttpApiConfig httpApiConfig)
                : base(httpApiConfig)
            {
            }

            /// <summary>
            /// 不释放
            /// </summary>
            public override void Dispose()
            {
            }
        }
    }
}
