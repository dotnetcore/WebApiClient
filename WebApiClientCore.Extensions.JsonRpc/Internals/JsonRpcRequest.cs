using System;
using System.Text.Json.Serialization;
using System.Threading;

namespace WebApiClientCore.Extensions.JsonRpc
{
    /// <summary>
    /// 表示JsonRpc的请求实体
    /// </summary>
    class JsonRpcRequest
    {  
        /// <summary>
        /// id值
        /// </summary>
        private static int @id = 0;

        /// <summary>
        /// jsonrpc
        /// 2.0
        /// </summary>
        [JsonPropertyName("jsonrpc")]
        public string JsonRpc { get; set; } = "2.0";

        /// <summary>
        /// 请求的方法名
        /// </summary>
        [JsonPropertyName("method")]
        public string Method { get; set; } = string.Empty;

        /// <summary>
        /// 请求的参数数组
        /// </summary>
        [JsonPropertyName("params")]
        public object Params { get; set; } = Array.Empty<object>();

        /// <summary>
        /// 请求的id
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; } = Interlocked.Increment(ref id);
    }
}
