using Mono.Cecil;
using System.Collections.Generic;

namespace WebApiClient.AOT.Task
{
    /// <summary>
    /// TypeDefinition比较器
    /// </summary>
    class TypeDefinitionComparer : IEqualityComparer<TypeDefinition>
    {
        /// <summary>
        /// 获取唯一实例
        /// </summary>
        public static readonly TypeDefinitionComparer Instance = new TypeDefinitionComparer();

        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(TypeDefinition x, TypeDefinition y)
        {
            return true;
        }

        /// <summary>
        /// 返回哈希值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(TypeDefinition obj)
        {
            return obj.FullName.GetHashCode();
        }
    }
}
