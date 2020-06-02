namespace WebApiClientCore.HttpContents
{
    /// <summary>
    /// 表示json内容
    /// </summary>
    public class JsonContent : BufferContent
    {
        /// <summary>
        /// 获取对应的ContentType
        /// </summary>
        public static string MediaType => "application/json";

        /// <summary>
        /// json内容
        /// </summary> 
        public JsonContent()
            : base(MediaType)
        {
        }
    }
}
