using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace WebApiClientCore.Analyzers.HttpApi
{
    class ModifierDiagnostic : HttpApiDiagnostic
    {
        public override DiagnosticDescriptor Descriptor => Descriptors.ModifierDescriptor;

        public ModifierDiagnostic(HttpApiContext context)
            : base(context)
        {
        }

        protected override IEnumerable<Diagnostic> GetDiagnostics()
        {
            var isVisiable = this.Context.HttpApiSyntax.Modifiers.Any(item => "public".Equals(item.ValueText));
            if (isVisiable == false)
            {
                var location = this.Context.HttpApiSyntax.Identifier.GetLocation();
                yield return this.CreateDiagnostic(location);
            }
        }
    }
}
