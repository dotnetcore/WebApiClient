using NJsonSchema.CodeGeneration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace WebApiClientCore.OpenApi.SourceGenerator
{
    /// <summary>
    /// 表示c#代码
    /// 自动代码美化
    /// </summary>
    [DebuggerDisplay("TypeName = {TypeName}")]
    public class CSharpCode : CodeArtifact
    {
        /// <summary>
        /// c#代码
        /// </summary>
        /// <param name="codeArtifact">源代码</param>
        public CSharpCode(CodeArtifact codeArtifact)
            : this(TransformCode(codeArtifact.Code), codeArtifact.TypeName, codeArtifact.Type)
        {
        }

        /// <summary>
        /// c#代码
        /// </summary>
        /// <param name="source">源代码</param>
        /// <param name="typeName">类型名称</param>
        /// <param name="type">类型分类</param>
        public CSharpCode(string source, string typeName, CodeArtifactType type)
            : base(typeName, type, CodeArtifactLanguage.CSharp, CodeArtifactCategory.Undefined, Pretty(source))
        {
        }

        /// <summary>
        /// 获取所有行
        /// </summary>
        public IEnumerable<string> Lines
        {
            get => GetLines(this.Code);
        }

        /// <summary>
        /// 转换为字符串代码
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Code;
        }

        /// <summary>
        /// 转换代码
        /// 将NSwag生成的模型代码转换为WebApiClient的模型代码
        /// </summary>
        /// <param name="nswagCode"></param>
        /// <returns></returns>
        private static string TransformCode(string nswagCode)
        {
            var builder = new StringBuilder();
            var lines = GetLines(nswagCode);

            foreach (var line in lines)
            {
                if (line.Contains("System.CodeDom.Compiler.GeneratedCode"))
                {
                    continue;
                }

                if (line.Contains("Newtonsoft.Json.JsonConverter"))
                {
                    continue;
                }

                var match = new Regex("(?<=Newtonsoft.Json.JsonProperty\\(\")\\w+(?=\")").Match(line);
                if (match.Success == true)
                {
                    builder.AppendLine($"[JsonPropertyName(\"{match.Value}\")]");
                    continue;
                }

                var cleaned = line
                    .Replace("partial class", "class")
                    .Replace("System.Collections.Generic.", null)
                    .Replace("System.Runtime.Serialization.",null)
                    .Replace("System.ComponentModel.DataAnnotations.", null);

                builder.AppendLine(cleaned);
            }

            return builder.ToString();
        }

        /// <summary>
        /// 美化
        /// </summary>
        /// <param name="code">源代码</param>
        /// <returns></returns>
        private static string Pretty(string code)
        {
            if (code == null)
            {
                return null;
            }

            var tab = 0;
            var builder = new StringBuilder();
            var compactCode = Compact(code);

            foreach (var line in GetLines(compactCode))
            {
                var cTab = tab;
                if (line == "{")
                {
                    tab = tab + 1;
                }
                else if (line == "}")
                {
                    cTab = tab - 1;
                    tab = tab - 1;
                }

                var isEndMethod = line.EndsWith(");");
                var isEndEnum = Regex.IsMatch(line, @"=\s*\d+\s*,");
                var isEndProperty = line.EndsWith("{ get; set; }");

                var prefix = string.Empty.PadRight(cTab * 4, ' ');
                var suffix = isEndMethod || isEndEnum || isEndProperty ?
                    Environment.NewLine :
                    null;

                builder.AppendLine($"{prefix}{line}{suffix}");
            }
            return builder.ToString().Trim();
        }

        /// <summary>
        /// 使代码紧凑
        /// </summary>
        /// <param name="code">源代码</param>
        /// <returns></returns>
        private static string Compact(string code)
        {
            if (code == null)
            {
                return null;
            }

            var builder = new StringBuilder();
            foreach (var line in GetLines(code))
            {
                var spaceTrim = Regex.Replace(line.Trim(), @"(?<=\().*(?=\))", m => m.Value.Trim());
                builder.AppendLine(spaceTrim);
            }

            var rn = Environment.NewLine;
            var trimCode = builder.ToString().Trim();
            return Regex.Replace(trimCode, $@"{rn}\s*{rn}", rn);
        }

        /// <summary>
        /// 返回所有行
        /// </summary>
        /// <param name="code">源代码</param>
        /// <returns></returns>
        private static IEnumerable<string> GetLines(string code)
        {
            if (code == null)
            {
                yield break;
            }

            using (var reader = new StringReader(code))
            {
                while (reader.Peek() >= 0)
                {
                    yield return reader.ReadLine();
                }
            }
        }
    }
}
