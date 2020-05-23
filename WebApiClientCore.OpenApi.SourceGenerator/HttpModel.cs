using NJsonSchema.CodeGeneration;

namespace WebApiClientCore.OpenApi.SourceGenerator
{
    /// <summary>
    /// 表示WebApiClient的模型描述
    /// </summary>
    public class HttpModel : CSharpCode
    {
        /// <summary>
        /// 获取使用的命名空间
        /// </summary>
        public string NameSapce { get; }

        /// <summary>
        /// WebApiClient的模型描述
        /// </summary>
        /// <param name="codeArtifact">源代码</param>
        /// <param name="nameSpace">命名空间</param>
        public HttpModel(CodeArtifact codeArtifact, string nameSpace)
           : base(codeArtifact)
        {
            this.NameSapce = nameSpace;
        }

        /// <summary>
        /// 转换为完整的代码
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var cshtml = CSharpHtml.Views<HttpModel>();
            var source = cshtml.RenderText(this);
            return new CSharpCode(source, this.TypeName, this.Type).ToString();
        }
    }
}
