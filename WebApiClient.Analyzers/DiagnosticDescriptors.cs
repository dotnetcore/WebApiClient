using Microsoft.CodeAnalysis;

namespace WebApiClient.Analyzers
{
    /// <summary>
    /// 诊断描述器
    /// </summary>
    static class DiagnosticDescriptors
    {
        /// <summary>
        /// 特性诊断描述器
        /// </summary>
        public static DiagnosticDescriptor AttributeDescriptor { get; }
           = Rule("WA1001", "不匹配的特性构造函数", "不支持特性的此构造函数，请使用其它构造函数");

        /// <summary>
        /// 方法返回类型诊断描述器
        /// </summary>
        public static DiagnosticDescriptor ReturnTypeDescriptor { get; }
            = Rule("WA1002", "不支持的返回类型", "返回类型必须为ITask<>或Task<>");

        /// <summary>
        /// 引用参数诊断描述器
        /// </summary>
        public static DiagnosticDescriptor RefParameterDescriptor { get; }
            = Rule("WA1003", "不支持的ref/out修饰", "参数不支持ref/out等修饰");

        /// <summary>
        /// 非方法声明诊断描述器
        /// </summary>
        public static DiagnosticDescriptor NotMethodDefindDescriptor { get; }
            = Rule("WA1004", "不支持的非方法声明", "不支持的非方法声明，只允许方法的声明");

        /// <summary>
        /// 创建诊断描述器
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="helpLinkUri"></param>
        /// <returns></returns>
        private static DiagnosticDescriptor Rule(string id, string title, string message, string helpLinkUri = null)
        {
            const string category = "Error";
            if (string.IsNullOrEmpty(helpLinkUri) == true)
            {
                helpLinkUri = "https://github.com/dotnetcore/WebApiClient/wiki/WebApiClient%E5%9F%BA%E7%A1%80";
            }
            return new DiagnosticDescriptor(id, title, message, category, DiagnosticSeverity.Error, true, helpLinkUri: helpLinkUri);
        }

        /// <summary>
        /// 转换为诊断结果
        /// </summary>
        /// <param name="diagnosticDescriptor"></param>
        /// <param name="location"></param>
        /// <param name="messageArgs"></param>
        /// <returns></returns>
        public static Diagnostic ToDiagnostic(this DiagnosticDescriptor diagnosticDescriptor, Location location, params object[] messageArgs)
        {
            return Diagnostic.Create(diagnosticDescriptor, location, messageArgs);
        }
    }
}
