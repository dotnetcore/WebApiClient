using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag;
using NSwag.CodeGeneration;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.CSharp.Models;
using NSwag.CodeGeneration.OperationNameGenerators;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebApiClientCore.OpenApi.SourceGenerator
{
    /// <summary>
    /// 表示WebApiClientCore接口设置模型
    /// </summary>
    public class HttpApiSettings : CSharpControllerGeneratorSettings
    {
        /// <summary>
        /// 获取或设置命名空间
        /// </summary>
        public string NameSpace { get; set; } = "WebApiClientCore";

        /// <summary>
        /// WebApiClient接口设置模型
        /// </summary>
        public HttpApiSettings()
        {
            this.ResponseArrayType = "List";
            this.ResponseDictionaryType = "Dictionary";
            this.ParameterArrayType = "IEnumerable";
            this.ParameterDictionaryType = "IDictionary"; 

            this.OperationNameGenerator = new OperationNameProvider();
            this.ParameterNameGenerator = new ParameterNameProvider();
            this.CSharpGeneratorSettings.TypeNameGenerator = new TypeNameProvider();
            this.CSharpGeneratorSettings.ClassStyle = CSharpClassStyle.Poco;
            this.CSharpGeneratorSettings.GenerateJsonMethods = false;
            this.RouteNamingStrategy = CSharpControllerRouteNamingStrategy.OperationId;
        }

        /// <summary>
        /// 方法名称提供者
        /// </summary>
        private class OperationNameProvider : MultipleClientsFromOperationIdOperationNameGenerator
        {
            /// <summary>
            /// 获取方法对应的类名
            /// </summary>
            /// <param name="document"></param>
            /// <param name="path"></param>
            /// <param name="httpMethod"></param>
            /// <param name="operation"></param>
            /// <returns></returns>
            public override string GetClientName(OpenApiDocument document, string path, string httpMethod, OpenApiOperation operation)
            {
                return operation.Tags.FirstOrDefault();
            }
        }

        /// <summary>
        /// 参数名提供者
        /// </summary>
        private class ParameterNameProvider : IParameterNameGenerator
        {
            /// <summary>
            /// 生成参数名
            /// </summary>
            /// <param name="parameter"></param>
            /// <param name="allParameters"></param>
            /// <returns></returns>
            public string Generate(OpenApiParameter parameter, IEnumerable<OpenApiParameter> allParameters)
            {
                if (string.IsNullOrEmpty(parameter.Name))
                {
                    return "unnamed";
                }

                var variableName = CamelCase(parameter.Name
                    .Replace("-", "_")
                    .Replace(".", "_")
                    .Replace(" ", null)
                    .Replace("$", string.Empty)
                    .Replace("[", string.Empty)
                    .Replace("]", string.Empty));

                if (allParameters.Count(p => p.Name == parameter.Name) > 1)
                    return variableName + parameter.Kind;

                return variableName;
            }


            /// <summary>
            /// 骆驼命名
            /// </summary>
            /// <param name="name">名称</param>
            /// <returns></returns>
            private static string CamelCase(string name)
            {
                if (string.IsNullOrEmpty(name) || char.IsUpper(name[0]) == false)
                {
                    return name;
                }

                var charArray = name.ToCharArray();
                for (int i = 0; i < charArray.Length; i++)
                {
                    if (i == 1 && char.IsUpper(charArray[i]) == false)
                    {
                        break;
                    }

                    var hasNext = (i + 1 < charArray.Length);
                    if (i > 0 && hasNext && !char.IsUpper(charArray[i + 1]))
                    {
                        if (char.IsSeparator(charArray[i + 1]))
                        {
                            charArray[i] = char.ToLowerInvariant(charArray[i]);
                        }
                        break;
                    }
                    charArray[i] = char.ToLowerInvariant(charArray[i]);
                }
                return new string(charArray);
            }
        }

        /// <summary>
        /// 类型名称提供者
        /// </summary>
        private class TypeNameProvider : DefaultTypeNameGenerator
        {
            public override string Generate(JsonSchema schema, string typeNameHint, IEnumerable<string> reservedTypeNames)
            {
                var prettyName = PrettyName(typeNameHint);
                var typeName = base.Generate(schema, prettyName, reservedTypeNames);
                return typeName;
            }

            /// <summary>
            /// 美化类型名称
            /// </summary>
            /// <param name="name">名称</param>
            /// <returns></returns>
            private static string PrettyName(string name)
            {
                if (string.IsNullOrEmpty(name) == true)
                {
                    return name;
                }

                if (name.Contains("[]") == true)
                {
                    name = name.Replace("[]", "Array");
                }

                var matchs = Regex.Matches(name, @"\W");
                if (matchs.Count == 0 || matchs.Count % 2 > 0)
                {
                    return name;
                }

                var index = -1;
                return Regex.Replace(name, @"\W", m =>
                {
                    index = index + 1;
                    return index < matchs.Count / 2 ? "Of" : null;
                });
            }
        }
    }
}
