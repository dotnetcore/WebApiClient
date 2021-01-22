using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace WebApiClientCore.Analyzers.Diagnostics
{
    /// <summary>
    /// public修饰符号诊断
    /// </summary>
    sealed class ModifierDiagnostic : HttpApiDiagnostic
    {
        public override DiagnosticDescriptor Descriptor => Descriptors.ModifierDescriptor;

        /// <summary>
        /// public修饰符号诊断
        /// </summary>
        /// <param name="context"></param>
        public ModifierDiagnostic(HttpApiContext context)
            : base(context)
        {
        }

        protected override IEnumerable<Diagnostic?> GetDiagnostics()
        {
            var syntax = this.Context.InterfaceSyntax;
            if (syntax == null)
            {
                yield break;
            }

            var isVisiable = syntax.Modifiers.Any(item => "public".Equals(item.ValueText));
            if (isVisiable == false)
            {
                var location = syntax.Identifier.GetLocation();
                yield return this.CreateDiagnostic(location);
            }
        }
    }
}
