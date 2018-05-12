using System;

namespace WebApiClient.Parameterables
{
    /// <summary>
    /// 表示将自身作为请求的Bearer体系授权
    /// </summary>
    public class BearerToken : Authorization
    {
        /// <summary>
        /// Bearer体系授权信息
        /// </summary>
        /// <param name="token">令牌</param>
        /// <exception cref="ArgumentNullException"></exception>
        public BearerToken(string token)
            : base("Bearer", token)
        {
        }
    }
}
