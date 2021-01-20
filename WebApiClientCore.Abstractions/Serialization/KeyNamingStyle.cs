namespace WebApiClientCore.Serialization
{
    /// <summary>
    /// 键名风格
    /// </summary>
    public enum KeyNamingStyle : byte
    {
        /// <summary>
        /// 只包含最后一级属性名的短键名风格
        /// </summary>
        ShortName,

        /// <summary>
        /// 包含多级属性名的完整键名风格
        /// </summary>
        FullName,

        /// <summary>
        /// 同时包含根与多级属性名的完整键名风格
        /// </summary>
        FullNameWithRoot
    }
}
