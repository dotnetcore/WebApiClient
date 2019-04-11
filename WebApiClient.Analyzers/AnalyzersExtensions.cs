using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebApiClient.Analyzers
{
    /// <summary>
    /// 提供分析扩展
    /// </summary>
    static class AnalyzersExtensions
    {
        /// <summary>
        /// 获取HttpApi方法
        /// </summary>
        /// <param name="syntaxNodeContext"></param>
        /// <param name="webApiContext">WebApi上下文</param>
        /// <returns></returns>
        public static IEnumerable<IMethodSymbol> GetHttpApiMethodSymbols(this SyntaxNodeAnalysisContext syntaxNodeContext, WebApiContext webApiContext)
        {
            var interfaceDeclaration = syntaxNodeContext.Node as InterfaceDeclarationSyntax;
            if (interfaceDeclaration == null || interfaceDeclaration.BaseList == null)
            {
                yield break;
            }

            foreach (var baseType in interfaceDeclaration.BaseList.Types)
            {
                var type = syntaxNodeContext.SemanticModel.GetTypeInfo(baseType.Type).Type;
                if (type.Equals(webApiContext.IHttpApiType) == false)
                {
                    continue;
                }

                foreach (var member in interfaceDeclaration.Members)
                {
                    if (member.Kind() != SyntaxKind.MethodDeclaration)
                    {
                        continue;
                    }

                    var symbol = syntaxNodeContext.SemanticModel.GetDeclaredSymbol(member) as IMethodSymbol;
                    if (symbol != null)
                    {
                        yield return symbol;
                    }
                }
            }
        }


        /// <summary>
        /// 获取要诊断的特性
        /// </summary>
        /// <param name="methodSymbol">方法</param>
        /// <param name="webApiContext">WebApi上下文</param>
        /// <returns></returns>
        public static IEnumerable<AttributeData> GetDiagnosticAttributes(this IMethodSymbol methodSymbol, WebApiContext webApiContext)
        {
            var attributeCtorUsageType = webApiContext.AttributeCtorUsageAtributeType;
            foreach (var methodAttribuete in methodSymbol.GetAttributes())
            {
                if (methodAttribuete.IsDefind(AttributeCtorTargets.Method, webApiContext) == false)
                {
                    yield return methodAttribuete;
                }
            }

            foreach (var parameter in methodSymbol.Parameters)
            {
                foreach (var parameterAttribute in parameter.GetAttributes())
                {
                    if (parameterAttribute.IsDefind(AttributeCtorTargets.Parameter, webApiContext) == false)
                    {
                        yield return parameterAttribute;
                    }
                }
            }
        }

        /// <summary>
        /// 获取特性声明的AttributeCtorUsageAtribute是否声明了指定目标
        /// </summary>
        /// <param name="attributeData"></param>
        /// <param name="targets">指定目标</param>
        /// <param name="webApiContext">WebApi上下文</param>
        /// <returns></returns>
        private static bool IsDefind(this AttributeData attributeData, AttributeCtorTargets targets, WebApiContext webApiContext)
        {
            var ctorUsageAttribute = attributeData
                .AttributeConstructor
                .GetAttributes()
                .FirstOrDefault(item => item.AttributeClass.Equals(webApiContext.AttributeCtorUsageAtributeType));

            if (ctorUsageAttribute == null)
            {
                return true;
            }

            var ctorTargets = (AttributeCtorTargets)ctorUsageAttribute.ConstructorArguments[0].Value;
            return ctorTargets.HasFlag(targets);
        }
    }
}
