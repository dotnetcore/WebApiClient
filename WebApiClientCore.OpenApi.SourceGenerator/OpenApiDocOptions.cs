using CommandLine;
using CommandLine.Text;

namespace WebApiClientCore.OpenApi.SourceGenerator
{
    /// <summary>
    /// 表示命令选项
    /// </summary>
    public class OpenApiDocOptions
    {
        /// <summary>
        /// openApi的json本地文件路径或远程Uri地址
        /// </summary>
        [Option('o', "openapi", MetaValue = "OpenApi", Required = true, HelpText = "OpenApi的本地文件路径或远程Uri地址")]
        public string OpenApi { get; set; }

        /// <summary>
        /// 代码的命名空间
        /// </summary>
        [Option('n', "namespace", MetaValue = "Namespace", Required = false, HelpText = "代码的命名空间，如WebApiClientCore")]
        public string Namespace { get; set; }
    }
}
