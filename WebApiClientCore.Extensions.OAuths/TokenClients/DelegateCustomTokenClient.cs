using System;
using System.Threading.Tasks;

namespace WebApiClientCore.Extensions.OAuths.TokenClients
{
    /// <summary>
    /// 委托处理指定接口类型的自定义token客户端
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    class DelegateCustomTokenClient<THttpApi> : CustomTokenClient<THttpApi>
    {
        private readonly IServiceProvider serviceProvider;
        private readonly Func<IServiceProvider, Task<TokenResult?>> tokenRequest;

        /// <summary>
        /// 委托处理指定接口类型的自定义token客户端
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="tokenRequest"></param>
        public DelegateCustomTokenClient(IServiceProvider serviceProvider, Func<IServiceProvider, Task<TokenResult?>> tokenRequest)
        {
            this.serviceProvider = serviceProvider;
            this.tokenRequest = tokenRequest;
        }

        /// <summary>
        /// 请求获取token
        /// </summary>
        /// <returns></returns>
        public override Task<TokenResult?> RequestTokenAsync()
        {
            return this.tokenRequest(this.serviceProvider);
        }
    }
}
