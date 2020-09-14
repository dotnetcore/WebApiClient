namespace WebApiClientCore.Extensions.OAuths.TokenProviders
{
    /// <summary>
    /// 表示Client身份信息
    /// </summary>
    public class ClientCredentials : Credentials
    {
        /// <summary>
        /// 访问的范围
        /// </summary>        
        public string? Scope { get; set; }

        /// <summary>
        /// 扩展信息
        /// 简单模型或字典
        /// </summary>
        public object? Extra { get; set; }
    }
}
