using Microsoft.CodeAnalysis;

namespace WebApiClient.Analyzers
{
    /// <summary>
    /// Means call diagnostic
    /// </summary>
    abstract class InvocationDiagnostic
    {
        /// <summary>
        /// Get context
        /// </summary>
        protected InvocationContext Context { get; }

        /// <summary>
        /// Get diagnostic description
        /// </summary>
        public abstract DiagnosticDescriptor Descriptor { get; }


        /// <summary>
        /// Call diagnostic
        /// </summary>
        /// <param name="context">Calling context</param>
        public InvocationDiagnostic(InvocationContext context)
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
        /// Report diagnosis
        /// </summary>
        public void Report()
        {
            var diagnostic = this.GetDiagnostic();
            if (diagnostic != null)
            {
                this.Context.SyntaxNodeContext.ReportDiagnostic(diagnostic);
            }
        }

        /// <summary>
        /// Returned report diagnosis
        /// </summary>
        /// <returns></returns>
        protected abstract Diagnostic GetDiagnostic();
    }
}
