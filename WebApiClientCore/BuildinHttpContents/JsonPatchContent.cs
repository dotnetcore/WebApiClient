namespace WebApiClientCore
{
    /// <summary>
    /// 表示http请求的JsonPatch内容
    /// </summary>
    class JsonPatchContent : BufferContent
    {
        /// <summary>
        /// 获取对应的ContentType
        /// </summary>
        public static string MediaType => "application/json-patch+json";

        /// <summary>
        /// JsonPatch内容
        /// </summary>
        public JsonPatchContent()
            : base(MediaType)
        {
        }
    }
}
