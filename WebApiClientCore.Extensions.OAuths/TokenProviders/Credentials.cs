namespace WebApiClientCore.Extensions.OAuths.TokenProviders
{
    /// <summary>
    /// 表示身份信息
    /// </summary>
    public class Credentials
    {
        /// <summary>
        /// 客户端id
        /// </summary>
        public string? Client_id { get; set; }

        /// <summary>
        /// 客户端秘钥
        /// </summary>
        public string? Client_secret { get; set; }
    }
}
