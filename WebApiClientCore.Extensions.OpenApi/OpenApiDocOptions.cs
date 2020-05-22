using CommandLine;
using CommandLine.Text;

namespace WebApiClientCore.Extensions.OpenApi
{
    /// <summary>
    /// 表示命令选项
    /// </summary>
    public class OpenApiDocOptions
    {
        /// <summary>
        /// openApi的json本地文件路径或远程Uri地址
        /// </summary>
        [Option('o', "openapi", MetaValue = "OpenApi", Required = true, HelpText = "openApi的json本地文件路径或远程Uri地址")]
        public string OpenApi { get; set; }

        /// <summary>
        /// 代码的命名空间
        /// </summary>
        [Option('n', "namespace", MetaValue = "Namespace", Required = false, HelpText = "代码的命名空间，如WebApiClient.Swagger")]
        public string Namespace { get; set; }

        /// <summary>
        /// 返回使用帮助
        /// </summary>
        /// <returns></returns>
        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
