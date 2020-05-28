namespace WebApiClientCore.Serialization
{
    /// <summary>
    /// 键名风格
    /// </summary>
    public enum KeyNamingStyle : byte
    {
        /// <summary>
        /// 短名称
        /// </summary>
        ShortName,

        /// <summary>
        /// 完整名称
        /// </summary>
        FullName,

        /// <summary>
        /// 同时包含根的完整名称
        /// </summary>
        FullNameWithRoot
    }
}
