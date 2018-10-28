using System;
using System.Collections.Generic;

namespace WebApiClient
{
    /// <summary>
    /// 定义Api参数修饰特性提供者的行为
    /// </summary>
    public interface IApiParameterAttributeProvider
    {
        /// <summary>
        /// 返回参数特性
        /// </summary>
        /// <param name="parameterType">参数类型</param>
        /// <param name="defined">参数上声明的特性</param>
        /// <returns></returns>
        IEnumerable<IApiParameterAttribute> GetAttributes(Type parameterType, IEnumerable<IApiParameterAttribute> defined);
    }
}
