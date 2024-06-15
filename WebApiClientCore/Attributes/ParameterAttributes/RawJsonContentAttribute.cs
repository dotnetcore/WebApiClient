using WebApiClientCore.HttpContents;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示将参数的文本内容作为请求 json 内容
    /// </summary>
    public class RawJsonContentAttribute : RawStringContentAttribute
    {
        /// <summary>
        /// 将参数的文本内容作为请求 json 内容
        /// </summary>
        public RawJsonContentAttribute()
            : base(JsonContent.MediaType)
        {
        }
    }
}
