using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApiClientCore.Analyzers.SourceGenerator
{
    /// <summary>
    /// HttpApi的方法查找器
    /// </summary>
    static class HttpApiMethodFinder
    {
        /// <summary>
        /// 查找接口类型及其继承的接口的所有方法
        /// </summary>
        /// <param name="httpApiType">接口类型</param>  
        /// <returns></returns>
        public static IEnumerable<IMethodSymbol> FindApiMethods(INamedTypeSymbol httpApiType)
        {
            return httpApiType.AllInterfaces.Append(httpApiType)
                .SelectMany(item => item.GetMembers())
                .OfType<IMethodSymbol>()
                .Distinct(MethodEqualityComparer.Default);
        }

        /// <summary>
        /// 表示MethodInfo的相等比较器
        /// </summary>
        private class MethodEqualityComparer : IEqualityComparer<IMethodSymbol>
        {
            public static MethodEqualityComparer Default { get; } = new MethodEqualityComparer();

            public bool Equals(IMethodSymbol x, IMethodSymbol y)
            {
                if (x.Name != y.Name || !x.ReturnType.Equals(y.ReturnType, SymbolEqualityComparer.Default))
                {
                    return false;
                }

                var xParameterTypes = x.Parameters.Select(p => p.Type);
                var yParameterTypes = y.Parameters.Select(p => p.Type);
                return xParameterTypes.SequenceEqual(yParameterTypes, SymbolEqualityComparer.Default);
            }

            public int GetHashCode(IMethodSymbol obj)
            {
                var hashCode = 0;
                hashCode ^= obj.Name.GetHashCode();
                hashCode ^= obj.ReturnType.GetHashCode();
                foreach (var parameter in obj.Parameters)
                {
                    hashCode ^= parameter.Type.GetHashCode();
                }
                return hashCode;
            }
        }
    }
}
