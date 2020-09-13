namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 类型匹配模式
    /// </summary>
    public enum TypeMatchMode
    {
        /// <summary>
        /// 类型精确匹配
        /// </summary>
        TypeOnly,

        /// <summary>
        /// 类型或其父接口类型匹配
        /// </summary>
        TypeOrBaseTypes
    }
}
