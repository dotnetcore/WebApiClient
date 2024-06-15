#if NETSTANDARD2_1 || NET5_0
namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>
    /// Specifies that the attributed class, constructor, or method requires dynamic code.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Method, Inherited = false)]
    sealed class RequiresDynamicCodeAttribute : Attribute
    {
        /// <summary>
        /// Gets the message associated with the requirement for dynamic code.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiresDynamicCodeAttribute"/> class with the specified message.
        /// </summary>
        /// <param name="message">The message associated with the requirement for dynamic code.</param>
        public RequiresDynamicCodeAttribute(string message)
        {
            this.Message = message;
        }
    }
}
#endif