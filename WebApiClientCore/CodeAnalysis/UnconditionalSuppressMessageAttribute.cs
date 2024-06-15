#if NETSTANDARD2_1
namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>
    /// 表示一个用于取消对代码分析器规则的警告的特性
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    sealed class UnconditionalSuppressMessageAttribute : Attribute
    {
        /// <summary>
        /// 获取或设置警告的类别
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// 获取或设置要取消的检查标识符
        /// </summary>
        public string CheckId { get; }

        /// <summary>
        /// 获取或设置取消警告的理由
        /// </summary>
        public string? Justification { get; set; }

        /// <summary>
        /// 获取或设置消息的标识符
        /// </summary>
        public string? MessageId { get; set; }

        /// <summary>
        /// 获取或设置取消警告的范围
        /// </summary>
        public string? Scope { get; set; }

        /// <summary>
        /// 获取或设置取消警告的目标
        /// </summary>
        public string? Target { get; set; }

        /// <summary>
        /// 初始化 <see cref="UnconditionalSuppressMessageAttribute"/> 类的新实例
        /// </summary>
        /// <param name="category">警告的类别</param>
        /// <param name="checkId">要取消的检查标识符</param>
        public UnconditionalSuppressMessageAttribute(string category, string checkId)
        {
            this.Category = category;
            this.CheckId = checkId;
        }
    }
}
#endif