using Microsoft.CodeAnalysis.Text;

namespace WebApiClientCore.Analyzers.SourceGenerator
{
    /// <summary>
    /// 代码创建器接口
    /// </summary>
    interface ISourceTextBuilder
    {
        /// <summary>
        /// 获取文件名
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// 转换为SourceText
        /// </summary>
        /// <returns></returns>
        SourceText ToSourceText();
    }
}
