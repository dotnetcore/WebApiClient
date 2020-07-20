using RazorEngineCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace WebApiClientCore.OpenApi.SourceGenerator
{
    /// <summary>
    /// 提供视图模板操作
    /// </summary>
    static class CSharpHtml
    {
        /// <summary>
        /// 返回Views下的cshtml
        /// </summary>
        /// <param name="name">cshtml名称</param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <returns></returns>
        public static CSharpHtml<T> Views<T>()
        {
            return Views<T>(typeof(T).Name);
        }

        /// <summary>
        /// 返回Views下的cshtml
        /// </summary>
        /// <param name="name">cshtml名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <returns></returns>
        public static CSharpHtml<T> Views<T>(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var path = $"Views\\{name}";
            return new CSharpHtml<T>(path);
        }
    }

    /// <summary>
    /// 表示视图模板
    /// </summary>
    [DebuggerDisplay("{FilePath}")]
    class CSharpHtml<T>
    {
        /// <summary>
        /// 获取模板文件路径
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// 块元素
        /// </summary>
        public HashSet<string> BlockElements { get; }


        /// <summary>
        /// 视图模板
        /// </summary>
        /// <param name="path">cshtml文件路径</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public CSharpHtml(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                path = Path.ChangeExtension(path, ".cshtml");
            }

            path = Path.Combine(AppContext.BaseDirectory, path);
            if (File.Exists(path) == false)
            {
                throw new FileNotFoundException(path);
            }

            this.FilePath = Path.GetFullPath(path);
            this.BlockElements = new HashSet<string>(new[] { "p", "div" }, StringComparer.OrdinalIgnoreCase);
        }


        /// <summary>
        /// 返回视图Html
        /// </summary>
        /// <param name="model">模型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public string RenderHtml(T model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var content = File.ReadAllText(this.FilePath);
            return Razor.Compile(content).Run(model);
        }

        /// <summary>
        /// 返回视图文本
        /// </summary>
        /// <param name="model">模型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public string RenderText(T model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var html = this.RenderHtml(model);
            var doc = XDocument.Parse(html).Root;
            var builder = new StringBuilder();

            RenderText(doc, builder);
            return builder.ToString();
        }

        /// <summary>
        /// 装载元素的文本
        /// </summary>
        /// <param name="element"></param>
        /// <param name="builder"></param>
        private void RenderText(XElement element, StringBuilder builder)
        {
            if (element.HasElements == true)
            {
                foreach (var item in element.Elements())
                {
                    RenderText(item, builder);
                }
                return;
            }

            var text = element.Value?.Trim();
            if (string.IsNullOrEmpty(text) == true)
            {
                return;
            }

            if (this.BlockElements.Contains(element.Name.ToString()))
            {
                builder.AppendLine().Append(text);
                if (element.NextNode == null)
                {
                    builder.AppendLine();
                }
            }
            else
            {
                builder.Append(text);
                if (element.NextNode != null)
                {
                    builder.Append(" ");
                }
            }
        }


        /// <summary>
        /// 表示Rozor引擎
        /// </summary>
        static class Razor
        {
            /// <summary>
            /// 模板缓存
            /// </summary>
            private static readonly ConcurrentDictionary<string, IRazorEngineCompiledTemplate> templateCache = new ConcurrentDictionary<string, IRazorEngineCompiledTemplate>();

            /// <summary>
            /// 编译并执行
            /// </summary>
            /// <param name="content">模板名称</param> 
            /// <returns></returns>
            public static IRazorEngineCompiledTemplate Compile(string content)
            {
                return templateCache.GetOrAdd(content, c => new RazorEngine().Compile(content, builder =>
                {
                    builder.Inherits(typeof(HtmlTempate));
                    builder.AddAssemblyReference(typeof(NSwag.OpenApiSchema).Assembly);
                    builder.AddAssemblyReference(typeof(NJsonSchema.JsonSchema).Assembly);

                    builder.AddUsing("NSwag");
                    builder.AddUsing("System");
                    builder.AddUsing("WebApiClientCore.OpenApi.SourceGenerator");
                }));
            }
        }
    }
}