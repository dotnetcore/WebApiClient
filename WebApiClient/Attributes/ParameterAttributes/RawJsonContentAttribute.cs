namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示将参数的json文本内容作为请求内容
    /// </summary>
    public class RawJsonContentAttribute : RawStringContentAttribute
    {
        /// <summary>
        /// 将参数的json文本内容作为请求内容
        /// </summary>
        public RawJsonContentAttribute()
            : base(JsonContent.MediaType)
        {
        }
    }
}
