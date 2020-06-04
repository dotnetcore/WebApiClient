namespace WebApiClientCore.HttpContents
{
    /// <summary>
    /// 表示utf8的JsonPatch内容
    /// </summary>
    public class JsonPatchContent : BufferContent
    {
        /// <summary>
        /// 获取对应的ContentType
        /// </summary>
        public static string MediaType => "application/json-patch+json";

        /// <summary>
        /// utf8的JsonPatch内容
        /// </summary>
        public JsonPatchContent()
            : base(MediaType)
        {
        }
    }
}
