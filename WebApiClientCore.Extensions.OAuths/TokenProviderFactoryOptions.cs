using System;
using System.Collections.Generic;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 用于记录接口类型与TokenProviderService的映射关系
    /// </summary>
    sealed class TokenProviderFactoryOptions : Dictionary<Type, Type>
    {
        /// <summary>
        /// 登记映射
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <typeparam name="TTokenPrivder">提供者类型</typeparam>
        public void Register<THttpApi, TTokenPrivder>() where TTokenPrivder : ITokenProvider
        {
            var httpApiType = typeof(THttpApi);
            this[httpApiType] = typeof(TokenProviderService<THttpApi, TTokenPrivder>);
        }
    }
}
