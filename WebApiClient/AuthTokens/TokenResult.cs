using WebApiClient.DataAnnotations;

namespace WebApiClient.AuthTokens
{
    /// <summary>
    /// 表示Token结果
    /// </summary>
    public class TokenResult
    {
        /// <summary>
        /// access_token
        /// </summary>
        [AliasAs("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// id_token
        /// </summary>
        [AliasAs("id_token")]
        public string IdTken { get; set; }

        /// <summary>
        /// expires_in
        /// 过期时间戳(秒)
        /// </summary>
        [AliasAs("expires_in")]
        public long ExpiresIn { get; set; }

        /// <summary>
        /// token_type
        /// </summary>
        [AliasAs("token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// refresh_token
        /// </summary>
        [AliasAs("refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// error
        /// </summary>
        [AliasAs("error")]
        public string Error { get; set; }

        /// <summary>
        /// 返回是否成功
        /// </summary>
        public bool IsSuccess()
        {
            return string.IsNullOrEmpty(Error);
        }
    }
}