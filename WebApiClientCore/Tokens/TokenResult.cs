using System;
using System.Text.Json.Serialization;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore.Tokens
{
    /// <summary>
    /// 表示Token结果
    /// </summary>
    public class TokenResult
    {
        /// <summary>
        /// token创建时间
        /// </summary>
        private readonly DateTime createTime = DateTime.Now;

        /// <summary>
        /// access_token
        /// </summary>
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// id_token
        /// </summary>
        [JsonPropertyName("id_token")]
        public string IdToken { get; set; }

        /// <summary>
        /// expires_in
        /// 过期时间戳(秒)
        /// </summary>
        [JsonPropertyName("expires_in")]
        public long ExpiresIn { get; set; }

        /// <summary>
        /// token_type
        /// </summary>
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// refresh_token
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// error
        /// </summary>
        [JsonPropertyName("error")]
        public string Error { get; set; }

        /// <summary>
        /// 确保token成功
        /// </summary>
        /// <exception cref="HttpApiTokenException"></exception>
        public TokenResult EnsureSuccess()
        {
            if (this.IsSuccess() == true)
            {
                return this;
            }
            throw new HttpApiTokenException(this.Error);
        }

        /// <summary>
        /// 返回是否成功
        /// </summary>
        /// <returns></returns>
        public bool IsSuccess()
        {
            return string.IsNullOrEmpty(this.Error);
        }

        /// <summary>
        /// 返回是否已过期 
        /// </summary>
        /// <returns></returns>
        public bool IsExpired()
        {
            return DateTime.Now.Subtract(this.createTime) > TimeSpan.FromSeconds(this.ExpiresIn);
        }

        /// <summary>
        /// 返回token是否支持刷新
        /// </summary>
        /// <returns></returns>
        public bool CanRefresh()
        {
            return string.IsNullOrEmpty(this.RefreshToken) == false;
        }
    }
}