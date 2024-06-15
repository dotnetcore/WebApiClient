namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 表示 http 接口的 token 提供者服务
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    /// <typeparam name="TTokenProvider"></typeparam>
    sealed class TokenProviderService<THttpApi, TTokenProvider> : ITokenProviderService where TTokenProvider : ITokenProvider
    {
        /// <summary>
        /// 获取 token 提供者
        /// </summary>
        public ITokenProvider TokenProvider { get; }

        /// <summary>
        /// http 接口的 token 提供者服务
        /// </summary>
        /// <param name="tokenProvider">token 提供者</param>
        public TokenProviderService(TTokenProvider tokenProvider)
        {
            this.TokenProvider = tokenProvider;
        }

        /// <summary>
        /// 设置服务提供者的名称
        /// </summary>
        /// <param name="alias"></param>
        public void SetProviderName(string alias)
        {
            this.TokenProvider.Name = GetTokenProviderName(alias);
        }

        /// <summary>
        /// 获取服务提供者的名称
        /// </summary> 
        /// <returns></returns>
        public static string GetTokenProviderName(string alias)
        {
            var httpApiName = HttpApi.GetName(typeof(THttpApi), false);
            var providerName = HttpApi.GetName(typeof(TTokenProvider), false);

            var name = $"{providerName}+{httpApiName}";
            if (string.IsNullOrEmpty(alias) == false)
            {
                name = $"{name}+{alias}";
            }
            return name;
        }
    }
}
