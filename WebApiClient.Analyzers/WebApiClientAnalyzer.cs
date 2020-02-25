using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using WebApiClient.Analyzers.HttpApi;
using WebApiClient.Analyzers.Invocation;

namespace WebApiClient.Analyzers
{
    /// <summary>
    /// Represents WebApiClient diagnostic analyzer
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class WebApiClientAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// Get Supported Diagnostics
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    Descriptors.AttributeDescriptor,
                    Descriptors.ReturnTypeDescriptor,
                    Descriptors.RefParameterDescriptor,
                    Descriptors.NotMethodDefindedDescriptor,
                    Descriptors.GenericMethodDescriptor,
                    Descriptors.UriAttributeDescriptor,
                    Descriptors.HttpApiCreateDescriptor);
            }
        }

        /// <summary>
        /// initialization
        /// </summary>
        /// <param name="context">Context</param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(syntaxNodeContext =>
            {
                var httpApiContext = new HttpApiContext(syntaxNodeContext);
                if (httpApiContext.IsHtttApi == true)
                {
                    foreach (var item in this.GetHttpApiDiagnostics(httpApiContext))
                    {
                        item.Report();
                    }
                }
            }, SyntaxKind.InterfaceDeclaration);

            context.RegisterSyntaxNodeAction(syntaxContext =>
            {
                var invocationContext = new InvocationContext(syntaxContext);
                foreach (var item in this.GetInvocationDiagnostics(invocationContext))
                {
                    item.Report();
                }
            }, SyntaxKind.InvocationExpression);
        }

        /// <summary>
        /// Returns all HttpApi diagnostics
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private IEnumerable<HttpApiDiagnostic> GetHttpApiDiagnostics(HttpApiContext context)
        {
            yield return new AttributeDiagnostic(context);
            yield return new ReturnTypeDiagnostic(context);
            yield return new RefParameterDiagnostic(context);
            yield return new NotMethodDefindedDiagnostic(context);
            yield return new GenericMethodDiagnostic(context);
            yield return new UriAttributeDiagnostic(context);
        }

        /// <summary>
        /// Return all call diagnostics
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private IEnumerable<InvocationDiagnostic> GetInvocationDiagnostics(InvocationContext context)
        {
            yield return new HttpApiCreateDiagnostic(context);
        }
    }
}