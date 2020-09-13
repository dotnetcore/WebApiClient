using System.Threading.Tasks;

namespace WebApiClientCore.Extensions.OAuths.TokenProviders
{
    /// <summary>
    /// 类型别名的token提供者
    /// </summary>
    class TypeTokenProvider<THttpApi> : ITokenProvider<THttpApi>
    {
        /// <summary>
        /// token提供者
        /// </summary>
        private readonly ITokenProvider tokenProvider;

        /// <summary>
        /// 设置别名
        /// </summary>
        public string Name
        {
            set => this.tokenProvider.Name = value;
        }

        /// <summary>
        /// 类型别名的token提供者
        /// </summary> 
        /// <param name="factory"></param>
        public TypeTokenProvider(ITokenProviderFactory factory)
        {
            this.tokenProvider = factory.Create(typeof(THttpApi));
        }

        /// <summary>
        /// 强制清除token以支持下次获取到新的token
        /// </summary>
        public void ClearToken()
        {
            this.tokenProvider.ClearToken();
        }

        /// <summary>
        /// 获取token信息
        /// </summary> 
        /// <returns></returns>
        public Task<TokenResult> GetTokenAsync()
        {
            return this.tokenProvider.GetTokenAsync();
        }

        /// <summary>
        /// 转换为string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.tokenProvider.ToString();
        }
    }
}
