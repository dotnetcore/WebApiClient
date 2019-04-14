using Microsoft.CodeAnalysis;

namespace WebApiClient.Analyzers
{
    /// <summary>
    /// 表示调用诊断器
    /// </summary>
    abstract class InvocationDiagnostic
    {
        /// <summary>
        /// 获取上下文
        /// </summary>
        protected InvocationContext Context { get; }

        /// <summary>
        /// 获取诊断描述
        /// </summary>
        public abstract DiagnosticDescriptor Descriptor { get; }


        /// <summary>
        /// 调用诊断器
        /// </summary>
        /// <param name="context">调用上下文</param>
        public InvocationDiagnostic(InvocationContext context)
        {
            this.Context = context;
        }

        /// <summary>
        /// 创建诊断结果
        /// </summary>
        /// <param name="location"></param>
        /// <param name="messageArgs"></param>
        /// <returns></returns>
        protected Diagnostic CreateDiagnostic(Location location, params object[] messageArgs)
        {
            if (location == null)
            {
                return null;
            }
            return Diagnostic.Create(this.Descriptor, location, messageArgs);
        }


        /// <summary>
        /// 报告诊断结果
        /// </summary>
        public void Report()
        {
            var diagnostic = this.GetDiagnostic();
            if (diagnostic != null)
            {
                this.Context.SyntaxNodeContext.ReportDiagnostic(diagnostic);
            }
        }

        /// <summary>
        /// 返回的报告诊断
        /// </summary>
        /// <returns></returns>
        protected abstract Diagnostic GetDiagnostic();
    }
}
