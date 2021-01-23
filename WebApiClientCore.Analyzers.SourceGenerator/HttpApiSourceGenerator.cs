using Microsoft.CodeAnalysis;
using System.Linq;

namespace WebApiClientCore.Analyzers.SourceGenerator
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
                var builders = receiver
                    .GetHttpApiTypes(context.Compilation)
                    .Select(i => new HttpApiCodeBuilder(i))
                    .Distinct();

                foreach (var builder in builders)
                {
                    context.AddSource(builder.HttpApiTypeName, builder.ToSourceText());
                }
            }
        }
    }
}
