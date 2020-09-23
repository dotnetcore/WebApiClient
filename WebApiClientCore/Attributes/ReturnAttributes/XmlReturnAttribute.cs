using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApiClientCore.HttpContents;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示json内容的结果特性
    /// </summary>
    public class XmlReturnAttribute : ApiReturnAttribute
    {
        /// <summary>
        /// text/xml
        /// </summary>
        private static readonly MediaTypeHeaderValue textXml = new MediaTypeHeaderValue("text/xml");

        /// <summary>
        /// json内容的结果特性
        /// </summary>
        public XmlReturnAttribute()
            : base(new MediaTypeWithQualityHeaderValue(XmlContent.MediaType))
        {
        }

        /// <summary>
        /// json内容的结果特性
        /// </summary>
        /// <param name="acceptQuality">accept的质比</param>
        public XmlReturnAttribute(double acceptQuality)
            : base(new MediaTypeWithQualityHeaderValue(XmlContent.MediaType, acceptQuality))
        {
        }

        /// <summary>
        /// 指示响应的ContentType与AcceptContentType是否匹配
        /// 返回false则调用下一个ApiReturnAttribute来处理响应结果
        /// </summary>
        /// <param name="responseContentType">响应的ContentType</param>
        /// <returns></returns>
        protected override bool IsMatchAcceptContentType(MediaTypeHeaderValue? responseContentType)
        {
            return base.IsMatchAcceptContentType(responseContentType) || base.IsMatchAcceptContentType(textXml);
        }

        /// <summary>
        /// 设置强类型模型结果值
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override async Task SetResultAsync(ApiResponseContext context)
        {
            var resultType = context.ApiAction.Return.DataType.Type;
            context.Result = await context.XmlDeserializeAsync(resultType).ConfigureAwait(false);
        }
    }
}
