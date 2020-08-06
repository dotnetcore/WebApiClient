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
        public int Code { get; set; }

        /// <summary>
        /// 提示消息
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// 错误内容
        /// </summary>
        public object? Data { get; set; }
    }
}
