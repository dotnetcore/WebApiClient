using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace WebApiClient.Analyzers
{
    /// <summary>
    /// 表示WebApiClient的HttpApi接口
    /// </summary>
    class WebApiClientHtttApi
    {
        /// <summary>
        /// 语法节点上下文
        /// </summary>
        private readonly SyntaxNodeAnalysisContext syntaxNodeContext;

        /// <summary>
        /// 接口声明
        /// </summary>
        private readonly InterfaceDeclarationSyntax httpApiInterfaceSyntax;

        /// <summary>
        /// webApiClient上下文
        /// </summary>
        private readonly WebApiClientContext webApiClientContext;

        /// <summary>
        /// 获取是否为Http接口
        /// </summary>
        public bool IsHtttApiInterface { get; }

        /// <summary>
        /// WebApiClient的HttpApi接口
        /// </summary>
        /// <param name="syntaxNodeContext">语法节点上下文</param>
        public WebApiClientHtttApi(SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            this.syntaxNodeContext = syntaxNodeContext;
            this.httpApiInterfaceSyntax = syntaxNodeContext.Node as InterfaceDeclarationSyntax;
            this.webApiClientContext = new WebApiClientContext(syntaxNodeContext.Compilation);
            this.IsHtttApiInterface = this.GetIsHtttApiInterface();
        }

        /// <summary>
        /// 返回是否为HttpApi接口
        /// </summary>
        /// <returns></returns>
        private bool GetIsHtttApiInterface()
        {
            if (this.httpApiInterfaceSyntax == null || this.httpApiInterfaceSyntax.BaseList == null)
            {
                return false;
            }

            foreach (var baseType in this.httpApiInterfaceSyntax.BaseList.Types)
            {
                var type = this.syntaxNodeContext.SemanticModel.GetTypeInfo(baseType.Type).Type;
                if (type.Equals(this.webApiClientContext.IHttpApiType) == true)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 报告所有诊断
        /// </summary>
        public void ReportDiagnostic()
        {
            this.ReportDiagnosticOfAttributes();
            this.ReportDiagnosticOfReturnTypes();
            this.ReportDiagnosticOfRefParameters();
            this.ReportDiagnosticOfNotMethodDefinds();
        }

        /// <summary>
        /// 报告特性诊断
        /// </summary>
        public void ReportDiagnosticOfAttributes()
        {
            if (this.IsHtttApiInterface == false)
            {
                return;
            }

            var interfaceSymbol = this.syntaxNodeContext.SemanticModel.GetDeclaredSymbol(this.httpApiInterfaceSyntax);
            if (interfaceSymbol == null)
            {
                return;
            }

            var interfaceAttributes = this.GetInterfaceDiagnosticAttributes(interfaceSymbol);
            var methodAttributes = this.GetHttpApiMethodSymbols().SelectMany(item => this.GetMethodDiagnosticAttributes(item));

            foreach (var item in interfaceAttributes.Concat(methodAttributes))
            {
                var location = item.ApplicationSyntaxReference.GetSyntax().GetLocation();
                var diagnostic = DiagnosticDescriptors.AttributeDescriptor.ToDiagnostic(location);
                this.syntaxNodeContext.ReportDiagnostic(diagnostic);
            }
        }

        /// <summary>
        /// 报告返回类型诊断
        /// </summary>
        public void ReportDiagnosticOfReturnTypes()
        {
            if (this.IsHtttApiInterface == false)
            {
                return;
            }

            foreach (var method in this.GetHttpApiMethodSymbols())
            {
                var name = method.ReturnType.MetadataName;
                if (name != "Task`1" && name != "ITask`1")
                {
                    var methodSyntax = method.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as MethodDeclarationSyntax;
                    if (methodSyntax != null)
                    {
                        var location = methodSyntax.ReturnType.GetLocation();
                        var diagnostic = DiagnosticDescriptors.ReturnTypeDescriptor.ToDiagnostic(location);
                        this.syntaxNodeContext.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }

        /// <summary>
        /// 报告引用参数诊断
        /// </summary>
        public void ReportDiagnosticOfRefParameters()
        {
            if (this.IsHtttApiInterface == false)
            {
                return;
            }

            foreach (var method in this.GetHttpApiMethodSymbols())
            {
                foreach (var parameter in method.Parameters)
                {
                    if (parameter.RefKind != RefKind.None)
                    {
                        var parameterSyntax = parameter.DeclaringSyntaxReferences.First().GetSyntax() as ParameterSyntax;
                        var location = parameterSyntax.Modifiers.First().GetLocation();
                        var diagnostic = DiagnosticDescriptors.RefParameterDescriptor.ToDiagnostic(location);
                        this.syntaxNodeContext.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }

        /// <summary>
        /// 报告非方法声明诊断
        /// </summary>
        public void ReportDiagnosticOfNotMethodDefinds()
        {
            if (this.IsHtttApiInterface == false)
            {
                return;
            }

            foreach (var member in this.httpApiInterfaceSyntax.Members)
            {
                if (member.Kind() != SyntaxKind.MethodDeclaration)
                {
                    var location = member.GetLocation();
                    var diagnostic = DiagnosticDescriptors.NotMethodDefindDescriptor.ToDiagnostic(location);
                    this.syntaxNodeContext.ReportDiagnostic(diagnostic);
                }
            }
        }

        /// <summary>
        /// 获取HttpApi方法
        /// </summary>
        /// <returns></returns>
        private IEnumerable<IMethodSymbol> GetHttpApiMethodSymbols()
        {
            foreach (var member in this.httpApiInterfaceSyntax.Members)
            {
                if (member.Kind() != SyntaxKind.MethodDeclaration)
                {
                    continue;
                }

                var symbol = this.syntaxNodeContext.SemanticModel.GetDeclaredSymbol(member) as IMethodSymbol;
                if (symbol != null)
                {
                    yield return symbol;
                }
            }
        }

        /// <summary>
        /// 获取接口已诊断的特性
        /// </summary>
        /// <param name="interfaceSymbol">类型</param>
        /// <returns></returns>
        private IEnumerable<AttributeData> GetInterfaceDiagnosticAttributes(ITypeSymbol interfaceSymbol)
        {
            foreach (var attribuete in interfaceSymbol.GetAttributes())
            {
                if (this.CtorAttribueIsDefind(attribuete, AttributeCtorTargets.Interface) == false)
                {
                    yield return attribuete;
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
            foreach (var methodAttribuete in methodSymbol.GetAttributes())
            {
                if (this.CtorAttribueIsDefind(methodAttribuete, AttributeCtorTargets.Method) == false)
                {
                    yield return methodAttribuete;
                }
            }

            foreach (var parameter in methodSymbol.Parameters)
            {
                foreach (var parameterAttribute in parameter.GetAttributes())
                {
                    if (this.CtorAttribueIsDefind(parameterAttribute, AttributeCtorTargets.Parameter) == false)
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
        /// <returns></returns>
        private bool CtorAttribueIsDefind(AttributeData attributeData, AttributeCtorTargets targets)
        {
            var ctorAttributes = attributeData.AttributeConstructor?.GetAttributes();
            if (ctorAttributes.HasValue == false)
            {
                return true;
            }

            var ctorUsageAttribute = ctorAttributes.Value
                .FirstOrDefault(item => item.AttributeClass.Equals(this.webApiClientContext.AttributeCtorUsageAtributeType));

            if (ctorUsageAttribute == null)
            {
                return true;
            }

            var arg = ctorUsageAttribute.ConstructorArguments.FirstOrDefault();
            if (arg.IsNull == true)
            {
                return true;
            }

            var ctorTargets = (AttributeCtorTargets)arg.Value;
            return ctorTargets.HasFlag(targets);
        }
    }
}
