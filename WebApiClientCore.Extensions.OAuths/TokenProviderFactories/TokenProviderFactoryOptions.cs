using System;
using System.Collections.Generic;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// token提供者工厂选项
    /// 用于记录接口别名与接口提供者类型的映射
    /// </summary>
    class TokenProviderFactoryOptions : Dictionary<string, Type>
    {
        /// <summary>
        /// 登录映射
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <typeparam name="TTokenPrivder">提供者类型</typeparam>
        public void Register<THttpApi, TTokenPrivder>() where TTokenPrivder : ITokenProvider
        {
            var name = HttpApi.GetName<THttpApi>();
            this[name] = typeof(TTokenPrivder);
        }
    }
}
