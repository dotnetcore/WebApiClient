#if NETSTANDARD2_1
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Method, Inherited = false)]
    sealed class RequiresUnreferencedCodeAttribute : Attribute
    {
        /// <summary>
        /// 获取或设置对于未引用代码的要求的消息。
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// 初始化 <see cref="RequiresUnreferencedCodeAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="message">对于未引用代码的要求的消息。</param>
        public RequiresUnreferencedCodeAttribute(string message)
        {
            this.Message = message;
        }
    }
}
#endif