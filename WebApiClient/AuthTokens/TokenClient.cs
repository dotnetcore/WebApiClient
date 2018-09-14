using System;

namespace WebApiClient.AuthTokens
{
    /// <summary>
    /// 提供创建ITokenClient的实例
    /// </summary>
    public static class TokenClient
    {
        /// <summary>
        /// 返回创建ITokenClient的实例
        /// </summary>
        /// <param name="tokenEndpoint">提供Token获取的Url节点</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        /// <returns></returns>
        [Obsolete("此方法已过时，请使用Create方法替代", false)]
        public static ITokenClient Get(string tokenEndpoint)
        {
            return Create(tokenEndpoint);
        }

        /// <summary>
        /// 返回创建ITokenClient的实例
        /// </summary>
        /// <param name="tokenEndpoint">提供Token获取的Url节点</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        /// <returns></returns>
        [Obsolete("此方法已过时，请使用Create方法替代", false)]
        public static ITokenClient Get(Uri tokenEndpoint)
        {
            return Create(tokenEndpoint);
        }

        /// <summary>
        /// 返回创建ITokenClient的实例
        /// </summary>
        /// <param name="tokenEndpoint">提供Token获取的Url节点</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        /// <returns></returns>
        public static ITokenClient Create(Uri tokenEndpoint)
        {
            if (tokenEndpoint == null)
            {
                throw new ArgumentNullException(nameof(tokenEndpoint));
            }
            return Create(tokenEndpoint.ToString());
        }

        /// <summary>
        /// 返回创建ITokenClient的实例
        /// </summary>
        /// <param name="tokenEndpoint">提供Token获取的Url节点</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        /// <returns></returns>
        public static ITokenClient Create(string tokenEndpoint)
        {
            if (string.IsNullOrEmpty(tokenEndpoint))
            {
                throw new ArgumentNullException(nameof(tokenEndpoint));
            }
            return HttpApiClient.Create<ITokenClient>(tokenEndpoint);
        }
    }
}
