using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.CSharp.Models;
using System.Text.RegularExpressions;

namespace WebApiClientCore.OpenApi.SourceGenerator
{
    /// <summary>
    /// 表示WebApiClient的请求方法数据模型
    /// </summary>
    public class HttpApiMethod : CSharpOperationModel
    {
        /// <summary>
        /// WebApiClient的请求方法数据模型
        /// </summary>
        /// <param name="operation">Swagger操作</param>
        /// <param name="settings">设置项</param>
        /// <param name="generator">代码生成器</param>
        /// <param name="resolver">语法解析器</param>
        public HttpApiMethod(OpenApiOperation operation, CSharpGeneratorBaseSettings settings, CSharpGeneratorBase generator, CSharpTypeResolver resolver)
            : base(operation, settings, generator, resolver)
        {
        }

        /// <summary>
        /// 获取方法的返回类型
        /// 默认使用ITask
        /// </summary>
        public override string ResultType
        {
            get
            {
                return this.SyncResultType == "void"
                    ? "Task"
                    : $"ITask<{this.SyncResultType}>";
            }
        }

        /// <summary>
        /// 获取方法好友名称
        /// </summary>
        public override string ActualOperationName
        {
            get
            {
                var name = base.ActualOperationName;
                if (Regex.IsMatch(name, @"^\d") == false)
                {
                    return name;
                }

                name = Regex.Match(this.Id, @"\w*").Value;
                if (string.IsNullOrEmpty(name))
                {
                    name = "unnamed";
                }

                var names = name.ToCharArray();
                names[0] = char.ToUpper(names[0]);
                return new string(names);
            }
        }

        /// <summary>
        /// 解析参数名称
        /// 将文件参数声明为MulitpartFile
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        protected override string ResolveParameterType(OpenApiParameter parameter)
        {
            if (parameter.IsBinary || parameter.IsBinaryBodyParameter)
            {
                if (parameter.CollectionFormat == OpenApiParameterCollectionFormat.Multi && !parameter.ActualSchema.Type.HasFlag(JsonObjectType.Array))
                {
                    return "IEnumerable<FormDataFile>";
                }
                return "FormDataFile";
            }
            return base.ResolveParameterType(parameter);
        }
    }
}
