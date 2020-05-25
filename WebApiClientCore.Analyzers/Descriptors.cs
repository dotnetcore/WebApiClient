using Microsoft.CodeAnalysis;

namespace WebApiClientCore.Analyzers
{
    /// <summary>
    /// 诊断描述器
    /// </summary>
    static class Descriptors
    {
        /// <summary>
        /// 特性诊断描述器
        /// </summary>
        public static DiagnosticDescriptor AttributeDescriptor { get; }
           = Create("WA1001", Resx.WA1001_title, Resx.WA1001_message);

        /// <summary>
        /// 方法返回类型诊断描述器
        /// </summary>
        public static DiagnosticDescriptor ReturnTypeDescriptor { get; }
            = Create("WA1002", Resx.WA1002_title, Resx.WA1002_message);

        /// <summary>
        /// 引用参数诊断描述器
        /// </summary>
        public static DiagnosticDescriptor RefParameterDescriptor { get; }
            = Create("WA1003", Resx.WA1003_title, Resx.WA1003_message);

        /// <summary>
        /// 非方法声明诊断描述器
        /// </summary>
        public static DiagnosticDescriptor NotMethodDefindedDescriptor { get; }
            = Create("WA1004", Resx.WA1004_title, Resx.WA1004_message);

        /// <summary>
        /// 泛型方法诊断描述器
        /// </summary>
        public static DiagnosticDescriptor GenericMethodDescriptor { get; }
            = Create("WA1005", Resx.WA1005_title, Resx.WA1005_message);

        /// <summary>
        /// UriAttribute诊断描述器
        /// </summary>
        public static DiagnosticDescriptor UriAttributeDescriptor { get; }
            = Create("WA1006", Resx.WA1006_title, Resx.WA1006_message);


        /// <summary>
        /// 修饰符诊断描述器
        /// </summary>
        public static DiagnosticDescriptor ModifierDescriptor { get; }
            = Create("WA1007", Resx.WA1007_title, Resx.WA1007_message);

        /// <summary>
        /// 创建诊断描述器
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
                helpLinkUri = Resx.helpLinkUri;
            }
            return new DiagnosticDescriptor(id, title, message, category, level, true, helpLinkUri: helpLinkUri);
        }
    }
}
