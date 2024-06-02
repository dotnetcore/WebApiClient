using Microsoft.CodeAnalysis;

namespace WebApiClientCore.Analyzers.SourceGenerator
{
    static class SymbolExtensions
    {
        public static string GetFullName(this ISymbol symbol)
        {
            return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }
    }
}
