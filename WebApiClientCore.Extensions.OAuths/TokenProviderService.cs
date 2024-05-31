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
        /// 获取token提供者
        /// </summary>
        public ITokenProvider TokenProvider { get; }

        /// <summary>
        /// http接口的token提供者服务
        /// </summary>
        /// <param name="tokenProvider">token提供者</param>
        public TokenProviderService(TTokenProvider tokenProvider)
        {
            this.TokenProvider = tokenProvider;
        }

        /// <summary>
        /// 设置服务提供者的完整名称
        /// </summary>
        /// <param name="name"></param>
        public void SetProviderName(string name)
        {
            this.TokenProvider.Name = GetTokenProviderName(name);
        }

        /// <summary>
        /// 返回服务提供者的完整名称
        /// </summary> 
        /// <returns></returns>
        public static string GetTokenProviderName(string name)
        {
            var httpApiName = HttpApi.GetName(typeof(THttpApi), false);
            var providerName = HttpApi.GetName(typeof(TTokenProvider), false);
            var fullName = $"{providerName}+{httpApiName}";
            if (string.IsNullOrEmpty(name) == false)
            {
                fullName = $"{fullName}+{name}";
            }
            return fullName;
        }
    }
}
