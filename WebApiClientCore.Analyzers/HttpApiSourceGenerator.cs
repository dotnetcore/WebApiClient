using Microsoft.CodeAnalysis;
using System.Linq;
using WebApiClientCore.Analyzers.SourceGenerator;

namespace WebApiClientCore.Analyzers
{
    /// <summary>
    /// HttpApi代码生成器
    /// </summary>
    [Generator]
    public class HttpApiSourceGenerator : ISourceGenerator
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context"></param>
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new HttpApiSyntaxReceiver());
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="context"></param>
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is HttpApiSyntaxReceiver receiver)
            {   
                // System.Diagnostics.Debugger.Launch();
                var proxyClasses = receiver
                    .GetHttpApiTypes(context.Compilation)
                    .Select(i => new HttpApiProxyClass(i))
                    .Distinct()
                    .ToArray();

                if (proxyClasses.Length > 0)
                {
                    context.AddSource(HttpApiProxyClassStatic.FileName, HttpApiProxyClassStatic.ToSourceText());
                    context.AddSource(HttpApiProxyClassInitializer.FileName, HttpApiProxyClassInitializer.ToSourceText());

                    foreach (var proxyClass in proxyClasses)
                    {
                        context.AddSource(proxyClass.FileName, proxyClass.ToSourceText());
                    }
                }
            }
        }
    }
}
