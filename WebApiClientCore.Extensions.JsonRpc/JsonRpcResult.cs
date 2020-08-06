using System.Text.Json.Serialization;

namespace WebApiClientCore.Extensions.JsonRpc
{
    /// <summary>
    /// 表示JsonRpc的请求返回结果
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class JsonRpcResult<TResult>
    {
        /// <summary>
        /// 请求id
        /// </summary>
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        /// <summary>
        /// jsonrpc版本号
        /// </summary>
        [JsonPropertyName("jsonrpc")]
        public string JsonRpc { get; set; } = string.Empty;

        /// <summary>
        /// 结果值
        /// </summary>
#nullable disable
        [JsonPropertyName("result")]
        public TResult Result { get; set; }
#nullable enable

        /// <summary>
        /// 错误内容
        /// </summary>
        [JsonPropertyName("error")]
        public JsonRpcError? Error { get; set; }
    }
}

