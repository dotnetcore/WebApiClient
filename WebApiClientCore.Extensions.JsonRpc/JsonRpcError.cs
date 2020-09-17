using System.Text.Json.Serialization;

namespace WebApiClientCore.Extensions.JsonRpc
{
    /// <summary>
    /// 表示JsonRpc的错误内容
    /// </summary>
    public class JsonRpcError
    {
        /// <summary>
        /// 错误码
        /// </summary>
        [JsonPropertyName("code")]
        public int Code { get; set; }

        /// <summary>
        /// 提示消息
        /// </summary>
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        /// <summary>
        /// 错误内容
        /// </summary>
        [JsonPropertyName("data")]
        public object? Data { get; set; }
    }
}
