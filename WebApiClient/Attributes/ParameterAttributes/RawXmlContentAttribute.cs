namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示将参数的xml文本内容作为请求内容
    /// </summary>
    public class RawXmlContentAttribute : RawStringContentAttribute
    {
        /// <summary>
        /// 将参数的xml文本内容作为请求内容
        /// </summary>
        public RawXmlContentAttribute()
            : base(XmlContent.MediaType)
        {
        }
    }
}
