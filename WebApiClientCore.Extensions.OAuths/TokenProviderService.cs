using System.Linq;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 表示http接口的token提供者服务
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    /// <typeparam name="TTokenProvider"></typeparam>
    sealed class TokenProviderService<THttpApi, TTokenProvider> : ITokenProviderService where TTokenProvider : ITokenProvider
    {
        /// <summary>
        /// 获取提供者的别名
        /// </summary>
        public static string Name { get; } = GetTokenProviderName();

        /// <summary>
        /// 获取token提供者
        /// </summary>
        public ITokenProvider TokenProvider { get; }

        /// <summary>
        /// http接口的token提供者服务
        /// </summary>
        /// <param name="tokenProvider">token提供者</param>
        public TokenProviderService(TTokenProvider tokenProvider)
        {
            tokenProvider.Name = Name;
            this.TokenProvider = tokenProvider;
        }

        /// <summary>
        /// 返回服务提供者的别名
        /// </summary> 
        /// <returns></returns>
        private static string GetTokenProviderName()
        {
            var httpApiType = typeof(THttpApi);
            var name = httpApiType.Name;
            var providerName = typeof(TTokenProvider).Name;

            if (httpApiType.IsGenericType == false)
            {
                return $"{providerName}Of({name})";
            }

            var argumentsName = string.Join(",", httpApiType.GetGenericArguments().Select(i => i.Name));
            return $"{providerName}Of({name}<{argumentsName}>)";
        }
    }
}
