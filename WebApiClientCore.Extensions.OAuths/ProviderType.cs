namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// token提供者类型
    /// </summary>
    public enum ProviderType
    {
        /// <summary>
        /// OAuth的client_credentials授权模式token提供者
        /// </summary>
        ClientCredentials,

        /// <summary>
        /// OAuth的password授权模式token提供者
        /// </summary>
        PasswordClientCredentials,

        /// <summary>
        /// 自定义token提供者
        /// </summary>
        Custom,

        /// <summary>
        /// 其它token提供者
        /// </summary>
        Other
    }
}
