using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApiClientCore.Analyzers.HttpApi
{
    /// <summary>
    /// 表示特性构造函数诊断器
    /// </summary>
    class AttributeDiagnostic : HttpApiDiagnostic
    {
        /// <summary>
        /// /// <summary>
        /// 获取诊断描述
        /// </summary>
        /// </summary>
        public override DiagnosticDescriptor Descriptor => Descriptors.AttributeDescriptor;

        /// <summary>
        /// 特性构造函数诊断器
        /// </summary>
        /// <param name="context">上下文</param>
        public AttributeDiagnostic(HttpApiContext context)
            : base(context)
        {
        }

        /// <summary>
        /// 返回所有的报告诊断
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<Diagnostic> GetDiagnostics()
        {
            var interfaceSymbol = this.Context.SyntaxNodeContext.SemanticModel.GetDeclaredSymbol(this.Context.HttpApiSyntax);
            if (interfaceSymbol == null)
            {
                yield break;
            }

            var interfaceAttributes = this.GetInterfaceDiagnosticAttributes(interfaceSymbol);
            var methodAttributes = this.GetApiMethodSymbols().SelectMany(item => this.GetMethodDiagnosticAttributes(item));

            foreach (var item in interfaceAttributes.Concat(methodAttributes))
            {
                var location = item.ApplicationSyntaxReference?.GetSyntax()?.GetLocation();
                yield return this.CreateDiagnostic(location);
            }
        }


        /// <summary>
        /// 获取接口已诊断的特性
        /// </summary>
        /// <param name="interfaceSymbol">类型</param>
        /// <returns></returns>
        private IEnumerable<AttributeData> GetInterfaceDiagnosticAttributes(ITypeSymbol interfaceSymbol)
        {
            foreach (var attribute in interfaceSymbol.GetAttributes())
            {
                if (this.CtorAttributeIsDefind(attribute, AttributeTargets.Interface) == false)
                {
                    yield return attribute;
                }
            }
        }


        /// <summary>
        /// 获取方法已诊断的特性
        /// </summary>
        /// <param name="methodSymbol">方法</param>
        /// <returns></returns>
        private IEnumerable<AttributeData> GetMethodDiagnosticAttributes(IMethodSymbol methodSymbol)
        {
            foreach (var methodAttribute in methodSymbol.GetAttributes())
            {
                if (this.CtorAttributeIsDefind(methodAttribute, AttributeTargets.Method) == false)
                {
                    yield return methodAttribute;
                }
            }

            foreach (var parameter in methodSymbol.Parameters)
            {
                foreach (var parameterAttribute in parameter.GetAttributes())
                {
                    if (this.CtorAttributeIsDefind(parameterAttribute, AttributeTargets.Parameter) == false)
                    {
                        yield return parameterAttribute;
                    }
                }
            }
        }


        /// <summary>
        /// 获取特性声明的AttributeCtorUsageAttribute是否声明了指定目标
        /// </summary>
        /// <param name="attributeData"></param>
        /// <param name="targets">指定目标</param>
        /// <returns></returns>
        private bool CtorAttributeIsDefind(AttributeData attributeData, AttributeTargets targets)
        {
            var ctorAttributes = attributeData.AttributeConstructor?.GetAttributes();
            if (ctorAttributes.HasValue == false)
            {
                return true;
            }

            var ctorUsageAttribute = ctorAttributes.Value
                .FirstOrDefault(item => item.AttributeClass.Equals(this.Context.AttributeCtorUsageAttributeType));

            if (ctorUsageAttribute == null)
            {
                return true;
            }

            var arg = ctorUsageAttribute.ConstructorArguments.FirstOrDefault();
            if (arg.IsNull == true)
            {
                return true;
            }

            var ctorTargets = (AttributeTargets)arg.Value;
            return ctorTargets.HasFlag(targets);
        }
    }
}
