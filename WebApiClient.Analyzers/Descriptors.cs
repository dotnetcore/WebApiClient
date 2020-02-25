using Microsoft.CodeAnalysis;

namespace WebApiClient.Analyzers
{
    /// <summary>
    /// Diagnostic descriptor
    /// </summary>
    static class Descriptors
    {
        /// <summary>
        /// Characteristic diagnostic descriptor
        /// </summary>
        public static DiagnosticDescriptor AttributeDescriptor { get; }
           = Create("WA1001", "Mismatched property constructor", "This constructor does not support attributes，Please use another constructor");

        /// <summary>
        /// Method return type diagnostic descriptor
        /// </summary>
        public static DiagnosticDescriptor ReturnTypeDescriptor { get; }
            = Create("WA1002", "Unsupported return type", "The return type must be ITask<> or Task<>");

        /// <summary>
        /// Reference parameter diagnostic descriptor
        /// </summary>
        public static DiagnosticDescriptor RefParameterDescriptor { get; }
            = Create("WA1003", "Unsupported ref / out decoration", "Parameters do not support such modifications as ref/out");

        /// <summary>
        /// Non-method declaration diagnostic descriptor
        /// </summary>
        public static DiagnosticDescriptor NotMethodDefindedDescriptor { get; }
            = Create("WA1004", "Unsupported non-method declaration", "Unsupported non-method declarations, only method declarations are allowed");

        /// <summary>
        /// Generic method diagnostic descriptor
        /// </summary>
        public static DiagnosticDescriptor GenericMethodDescriptor { get; }
            = Create("WA1005", "Unsupported generic methods", "Declaring generic methods is not supported");

        /// <summary>
        /// UriAttribute diagnostic descriptor
        /// </summary>
        public static DiagnosticDescriptor UriAttributeDescriptor { get; }
            = Create("WA1006", "Unsupported decorated parameter index", "UriAttribute can only be modified on the first parameter");


        /// <summary>
        /// HttpApi.Create Api diagnostic descriptor
        /// </summary>
        public static DiagnosticDescriptor HttpApiCreateDescriptor { get; }
            = Create("WA2001", "Create function with caution",
                "HttpApi.Register/Resolve function is recommended，HttpApi.Create is generally used in conjunction with the HttpClient factory or for short link requests that are released after the request",
                level: DiagnosticSeverity.Warning,
                helpLinkUri: "https://github.com/dotnetcore/WebApiClient/wiki/WebApiClient%E8%BF%9B%E9%98%B6");



        /// <summary>
        /// Create a diagnostic descriptor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="helpLinkUri"></param>
        /// <returns></returns>
        private static DiagnosticDescriptor Create(string id, string title, string message, DiagnosticSeverity level = DiagnosticSeverity.Error, string helpLinkUri = null)
        {
            var category = level.ToString();
            if (string.IsNullOrEmpty(helpLinkUri) == true)
            {
                helpLinkUri = "https://github.com/dotnetcore/WebApiClient/wiki/WebApiClient%E5%9F%BA%E7%A1%80";
            }
            return new DiagnosticDescriptor(id, title, message, category, level, true, helpLinkUri: helpLinkUri);
        }
    }
}
