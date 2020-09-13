using System;
using System.Collections.Generic;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 用于记录接口类型与接口提供者类型的映射
    /// </summary>
    class HttpApiTokenProviderMap : Dictionary<Type, Type>
    {
        /// <summary>
        /// 登录映射
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <typeparam name="TTokenPrivder">提供者类型</typeparam>
        public void Register<THttpApi, TTokenPrivder>() where TTokenPrivder : ITokenProvider
        {
            this[typeof(THttpApi)] = typeof(TTokenPrivder);
        }
    }
}
