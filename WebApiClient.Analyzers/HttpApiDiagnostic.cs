using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace WebApiClient.Analyzers
{
    /// <summary>
    /// Represents the HttpApi diagnostic abstract class
    /// </summary>
    abstract class HttpApiDiagnostic
    {
        /// <summary>
        /// Get context
        /// </summary>
        protected HttpApiContext Context { get; }

        /// <summary>
        /// Get diagnostic description
        /// </summary>
        public abstract DiagnosticDescriptor Descriptor { get; }

        /// <summary>
        /// HttpApi Diagnostic
        /// </summary>
        /// <param name="context">Context</param>
        public HttpApiDiagnostic(HttpApiContext context)
        {
            this.Context = context;
        }

        /// <summary>
        /// Create diagnostic results
        /// </summary>
        /// <param name="location"></param>
        /// <param name="messageArgs"></param>
        /// <returns></returns>
        protected Diagnostic CreateDiagnostic(Location location, params object[] messageArgs)
        {
            if (location == null)
            {
                return null;
            }
            return Diagnostic.Create(this.Descriptor, location, messageArgs);
        }

        /// <summary>
        /// All methods that return HttpApi
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<IMethodSymbol> GetApiMethodSymbols()
        {
            foreach (var member in this.Context.HttpApiSyntax.Members)
            {
                if (member.Kind() != SyntaxKind.MethodDeclaration)
                {
                    continue;
                }

                if (this.Context.SyntaxNodeContext.SemanticModel.GetDeclaredSymbol(member) is IMethodSymbol symbol)
                {
                    yield return symbol;
                }
            }
        }

        /// <summary>
        /// Report diagnosis
        /// </summary>
        public void Report()
        {
            if (this.Context.IsHtttApi == false)
            {
                return;
            }

            foreach (var item in this.GetDiagnostics())
            {
                if (item != null)
                {
                    this.Context.SyntaxNodeContext.ReportDiagnostic(item);
                }
            }
        }

        /// <summary>
        /// Back to all report diagnostics
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<Diagnostic> GetDiagnostics();
    }
}
