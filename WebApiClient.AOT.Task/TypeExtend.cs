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
        /// 返回type的详细类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TypeInfo Detail(this Type type)
        {
            return type.GetTypeInfo();
        }

        /// <summary>
        /// 获取构造参数
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="types">参数类型</param>
        /// <returns></returns>
        public static ConstructorInfo GetConstructor(this Type type, Type[] types)
        {
            return type
                .Detail()
                .DeclaredConstructors
                .FirstOrDefault(item => item.GetParameters().Select(p => p.ParameterType).SequenceEqual(types));
        }
    }
}
#endif