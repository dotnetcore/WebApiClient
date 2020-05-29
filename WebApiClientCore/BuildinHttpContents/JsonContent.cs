namespace WebApiClientCore
{
    /// <summary>
    /// 表示http请求的json内容
    /// 从对象的json序列化到HttpContext发送过程为0拷贝
    /// </summary>
    class JsonContent : BufferContent
    {
        /// <summary>
        /// 获取对应的ContentType
        /// </summary>
        public static string MediaType => "application/json";

        /// <summary>
        /// http请求的json内容
        /// </summary> 
        public JsonContent()
            : base(MediaType)
        {
        }
    }
}
