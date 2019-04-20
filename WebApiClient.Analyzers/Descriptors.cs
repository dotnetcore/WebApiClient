using Microsoft.CodeAnalysis;

namespace WebApiClient.Analyzers
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
           = Create("WA1001", "不匹配的特性构造函数", "不支持特性的此构造函数，请使用其它构造函数");

        /// <summary>
        /// 方法返回类型诊断描述器
        /// </summary>
        public static DiagnosticDescriptor ReturnTypeDescriptor { get; }
            = Create("WA1002", "不支持的返回类型", "返回类型必须为ITask<>或Task<>");

        /// <summary>
        /// 引用参数诊断描述器
        /// </summary>
        public static DiagnosticDescriptor RefParameterDescriptor { get; }
            = Create("WA1003", "不支持的ref/out修饰", "参数不支持ref/out等修饰");

        /// <summary>
        /// 非方法声明诊断描述器
        /// </summary>
        public static DiagnosticDescriptor NotMethodDefindedDescriptor { get; }
            = Create("WA1004", "不支持的非方法声明", "不支持的非方法声明，只允许方法的声明");

        /// <summary>
        /// 泛型方法诊断描述器
        /// </summary>
        public static DiagnosticDescriptor GenericMethodDescriptor { get; }
            = Create("WA1005", "不支持的泛型方法", "不支持声明泛型方法");

        /// <summary>
        /// UriAttribute诊断描述器
        /// </summary>
        public static DiagnosticDescriptor UriAttributeDescriptor { get; }
            = Create("WA1006", "不支持的修饰的参数索引", "UriAttribute只能修饰于第一个参数");


        /// <summary>
        /// HttpApi.Create Api诊断描述器
        /// </summary>
        public static DiagnosticDescriptor HttpApiCreateDescriptor { get; }
            = Create("WA2001", "慎用的Create函数",
                "请慎用HttpApi.Create()函数，除非结合HttpClient工厂使用或者用于请求之后就释放的短链接请求",
                level: DiagnosticSeverity.Warning,
                helpLinkUri: "https://github.com/dotnetcore/WebApiClient/wiki/WebApiClient%E8%BF%9B%E9%98%B6");



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
                helpLinkUri = "https://github.com/dotnetcore/WebApiClient/wiki/WebApiClient%E5%9F%BA%E7%A1%80";
            }
            return new DiagnosticDescriptor(id, title, message, category, level, true, helpLinkUri: helpLinkUri);
        }
    }
}
