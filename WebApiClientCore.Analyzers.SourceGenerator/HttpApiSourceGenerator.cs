using Microsoft.CodeAnalysis;

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
                foreach (var httpApi in receiver.GetHttpApiTypes(context.Compilation))
                {
                    var builder = new HttpApiCodeBuilder(httpApi);
                    context.AddSource(builder.CtorName, builder.ToSourceText());
                }
            }
        }
    }
}
