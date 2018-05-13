#if NETCOREAPP1_0
using System;
using System.Linq;
using System.Reflection;

namespace WebApiClient.AOT.Task
{
    /// <summary>
    /// 类型扩展
    /// </summary>
    static class TypeExtend
    {
        /// <summary>
        /// 获取构造参数
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="types">参数类型</param>
        /// <returns></returns>
        public static ConstructorInfo GetConstructor(this Type type, Type[] types)
        {
            return type
                .GetTypeInfo()
                .DeclaredConstructors
                .FirstOrDefault(item => item.GetParameters().Select(p => p.ParameterType).SequenceEqual(types));
        }
    }
}
#endif