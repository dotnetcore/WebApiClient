using System;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// HttpApiOptions的扩展方法
    /// </summary>
    public static class HttpApiOptionsExtensions
    {
        /// <summary>
        /// OAuthTokenOptions在Properties字典中的键
        /// </summary>
        private const string TokenRefreshOptionsKey = "WebApiClientCore.OAuths.TokenRefreshOptions";

        /// <summary>
        /// 获取或设置OAuth Token刷新选项
        /// 此方法使用HttpApiOptions的Properties字典存储配置，不污染核心配置类
        /// </summary>
        /// <param name="options">HttpApi选项</param>
        /// <returns>OAuth Token刷新选项</returns>
        public static OAuthTokenOptions GetOAuthTokenOptions(this HttpApiOptions options)
        {
            if (!options.Properties.TryGetValue(TokenRefreshOptionsKey, out var value))
            {
                value = new OAuthTokenOptions();
                options.Properties[TokenRefreshOptionsKey] = value;
            }
            return (OAuthTokenOptions)value;
        }

        /// <summary>
        /// 配置OAuth Token刷新选项
        /// 此方法使用HttpApiOptions的Properties字典存储配置，不污染核心配置类
        /// </summary>
        /// <param name="options">HttpApi选项</param>
        /// <param name="configure">配置委托</param>
        /// <returns>HttpApi选项</returns>
        public static HttpApiOptions ConfigureOAuthToken(this HttpApiOptions options, Action<OAuthTokenOptions> configure)
        {
            var tokenOptions = options.GetOAuthTokenOptions();
            configure(tokenOptions);
            return options;
        }
    }
}
