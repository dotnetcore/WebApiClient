using System;
using System.Text.Json.Serialization;
using WebApiClientCore.Extensions.OAuths.Exceptions;

namespace WebApiClientCore.Extensions.OAuths
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
        public string? Access_token { get; set; }

        /// <summary>
        /// id_token
        /// </summary>
        [JsonPropertyName("id_token")]
        public string? Id_token { get; set; }

        /// <summary>
        /// expires_in
        /// access_token 过期时间戳(秒)
        /// </summary>
        [JsonPropertyName("expires_in")]
        public long Expires_in { get; set; }

        /// <summary>
        /// token_type
        /// </summary>
        [JsonPropertyName("token_type")]
        public string? Token_type { get; set; }

        /// <summary>
        /// refresh_token
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public string? Refresh_token { get; set; }

        /// <summary>
        /// refresh_expires_in
        /// refresh_token 过期时间戳(秒)
        /// </summary>
        [JsonPropertyName("refresh_expires_in")]
        public long? Refresh_expires_in { get; set; }

        /// <summary>
        /// error
        /// </summary>
        [JsonPropertyName("error")]
        public string? Error { get; set; }

        /// <summary>
        /// 确保token成功
        /// </summary>
        /// <exception cref="TokenException"></exception>
        public TokenResult EnsureSuccess()
        {
            if (this.IsSuccess() == true)
            {
                return this;
            }
            throw new TokenException(this.Error);
        }

        /// <summary>
        /// 返回是否成功
        /// </summary>
        /// <returns></returns>
        public virtual bool IsSuccess()
        {
            return string.IsNullOrEmpty(this.Access_token) == false;
        }

        /// <summary>
        /// 返回是否已过期
        /// </summary>
        /// <returns></returns>
        public virtual bool IsExpired()
        {
            return DateTime.Now.Subtract(this.createTime) > TimeSpan.FromSeconds(this.Expires_in);
        }

        /// <summary>
        /// 返回token是否支持刷新，需要同时满足：refresh_token有值，refresh_expires_in无值，若refresh_expires_in有值 必须当前没有过期。
        /// </summary>
        /// <returns></returns>
        public virtual bool CanRefresh()
        {
            return string.IsNullOrEmpty(this.Refresh_token) == false
                && (!this.Refresh_expires_in.HasValue
                    || (this.Refresh_expires_in.HasValue
                        && DateTime.Now.Subtract(this.createTime) < TimeSpan.FromSeconds(this.Refresh_expires_in.Value)));
        }
    }
}