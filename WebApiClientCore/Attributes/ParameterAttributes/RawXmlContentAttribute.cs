using WebApiClientCore.HttpContents;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示将参数的文本内容作为请求xml内容
    /// </summary>
    public class RawXmlContentAttribute : RawStringContentAttribute
    {
        /// <summary>
        /// 将参数的文本内容作为请求xml内容
        /// </summary>
        public RawXmlContentAttribute()
            : base(XmlContent.MediaType)
        {
        }
    }
}
