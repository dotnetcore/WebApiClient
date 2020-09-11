using Microsoft.Extensions.DependencyInjection;
using System;

namespace WebApiClientCore.Extensions.OAuths.HttpMessageHandlers
{
    /// <summary>
    /// 表示password授权方式token应用的http消息处理程序
    /// 需要注册services.AddPasswordCredentialsTokenProvider
    /// </summary>
    public class PasswordCredentialsTokenHandler : OAuthTokenHandler
    {
        /// <summary>
        /// password授权方式token应用的http消息处理程序
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        /// <param name="httpApiType">接口类型</param> 
        public PasswordCredentialsTokenHandler(IServiceProvider serviceProvider, Type httpApiType)
            : base(serviceProvider, httpApiType)
        {
        }

        /// <summary>
        /// 获取token提供者
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="httpApiType"></param>
        /// <returns></returns>
        protected override ITokenProvider GetTokenProvider(IServiceProvider serviceProvider, Type httpApiType)
        {
            var providerType = typeof(IPasswordCredentialsTokenProvider<>).MakeGenericType(httpApiType);
            return (ITokenProvider)serviceProvider.GetRequiredService(providerType);
        }
    }
}
