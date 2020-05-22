using RazorEngine.Configuration;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace WebApiClientCore.Extensions.OpenApi
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
    [DebuggerDisplay("{TemplateFile}")]
    class CSharpHtml<T> : ITemplateSource
    {
        /// <summary>
        /// 模板内容
        /// </summary>
        private readonly Lazy<string> template;

        /// <summary>
        /// 获取模板内容
        /// </summary>
        public string Template => this.template.Value;

        /// <summary>
        /// 获取模板文件路径
        /// </summary>
        public string TemplateFile { get; private set; }

        /// <summary>
        /// 块元素
        /// </summary>
        public HashSet<string> BlockElements { get; private set; }

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

            if (File.Exists(path) == false)
            {
                throw new FileNotFoundException(path);
            }

            this.template = new Lazy<string>(this.ReadTemplate);
            this.TemplateFile = Path.GetFullPath(path);
            this.BlockElements = new HashSet<string>(new[] { "p", "div" }, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 读取模板资源
        /// </summary>
        /// <returns></returns>
        private string ReadTemplate()
        {
            using (var stream = new FileStream(this.TemplateFile, FileMode.Open))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// 返回模板内容读取器
        /// </summary>
        /// <returns></returns>
        TextReader ITemplateSource.GetTemplateReader()
        {
            return new StringReader(this.Template);
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
            return Razor.RunCompile(this.TemplateFile, this, model);
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
            /// razor引擎
            /// </summary>
            private static readonly IRazorEngineService razor;

            /// <summary>
            /// 同步锁
            /// </summary>
            private static readonly object syncRoot = new object();

            /// <summary>
            /// 视图名称集合
            /// </summary>
            private static readonly HashSet<string> templateNames = new HashSet<string>();

            /// <summary>
            /// 视图模板
            /// </summary>
            static Razor()
            {
                var config = new TemplateServiceConfiguration
                {
                    Debug = true,
                    CachingProvider = new DefaultCachingProvider(t => { })
                };
                razor = RazorEngineService.Create(config);
            }

            /// <summary>
            /// 编译并执行
            /// </summary>
            /// <param name="name">模板名称</param>
            /// <param name="source">模板提供者</param>
            /// <param name="model">模型</param>
            /// <returns></returns>
            public static string RunCompile(string name, ITemplateSource source, object model)
            {
                if (model == null)
                {
                    throw new ArgumentNullException(nameof(model));
                }

                lock (syncRoot)
                {
                    if (templateNames.Add(name) == true)
                    {
                        razor.AddTemplate(name, source);
                        razor.Compile(name);
                    }
                }

                return razor.RunCompile(name, model.GetType(), model);
            }
        }
    }
}