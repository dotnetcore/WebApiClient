using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace WebApiClientCore.Analyzers.Providers
{
    /// <summary>
    /// public修饰符号诊断
    /// </summary>
    sealed class ModifierDiagnosticProvider : HttpApiDiagnosticProvider
    {
        public override DiagnosticDescriptor Descriptor => Descriptors.ModifierDescriptor;

        /// <summary>
        /// public修饰符号诊断
        /// </summary>
        /// <param name="context"></param>
        public ModifierDiagnosticProvider(HttpApiContext context)
            : base(context)
        {
        }

        public override IEnumerable<Diagnostic> CreateDiagnostics()
        {
            var syntax = this.Context.Syntax;
            var isVisible = syntax.Modifiers.Any(item => "public".Equals(item.ValueText));
            if (isVisible == false)
            {
                var location = syntax.Identifier.GetLocation();
                yield return this.CreateDiagnostic(location);
            }
        }
    }
}
