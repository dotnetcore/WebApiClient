using NJsonSchema.CodeGeneration;
using NSwag;
using NSwag.CodeGeneration.CSharp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WebApiClientCore.OpenApi.SourceGenerator
{
    /// <summary>
    /// 表示WebApiClient的接口数据模型
    /// </summary>
    [DebuggerDisplay("TypeName = {TypeName}")]
    public class HttpApi : CSharpControllerTemplateModel
    {
        public string NameSpace { get; } = "WebApiClientCore";

        /// <summary>
        /// 获取接口名称
        /// </summary>
        public string TypeName { get; }

        /// <summary>
        /// 获取文档描述
        /// </summary>
        public string Summary { get; }

        /// <summary>
        /// 获取是否有文档描述
        /// </summary>
        public bool HasSummary
        {
            get => string.IsNullOrEmpty(Summary) == false;
        }

        /// <summary>
        /// WebApiClient的接口数据模型
        /// </summary>
        /// <param name="className">类名</param>
        /// <param name="operations">swagger操作</param>
        /// <param name="document">swagger文档</param>
        /// <param name="settings">设置项</param>
        public HttpApi(string className, IEnumerable<CSharpOperationModel> operations, OpenApiDocument document, HttpApiSettings settings)
            : base(className, operations, document, settings)
        {
            var tag = document.Tags
                .FirstOrDefault(item => string.Equals(item.Name, className, StringComparison.OrdinalIgnoreCase));

            this.TypeName = $"I{this.Class}Api";
            this.Summary = tag?.Description;
            this.NameSpace = settings.NameSpace;
        }

        /// <summary>
        /// 转换为完整的代码
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var cshtml = CSharpHtml.Views<HttpApi>();
            var source = cshtml.RenderText(this);
            return new CSharpCode(source, this.TypeName, CodeArtifactType.Interface).ToString();
        }
    }
}
