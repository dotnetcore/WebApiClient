using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApiClientCore.HttpContents;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示 xml 内容的结果特性
    /// </summary>
    public class XmlReturnAttribute : ApiReturnAttribute
    {
        private static readonly HashSet<string> allowMediaTypes = new([XmlContent.MediaType, "text/xml"], StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// xml内容的结果特性
        /// </summary>
        public XmlReturnAttribute()
            : base(new MediaTypeWithQualityHeaderValue(XmlContent.MediaType))
        {
        }

        /// <summary>
        /// xml内容的结果特性
        /// </summary>
        /// <param name="acceptQuality">accept的质比</param>
        public XmlReturnAttribute(double acceptQuality)
            : base(new MediaTypeWithQualityHeaderValue(XmlContent.MediaType, acceptQuality))
        {
        }

        /// <summary>
        /// 指示响应的ContentType与AcceptContentType是否匹配
        /// 返回 false 则调用下一个ApiReturnAttribute来处理响应结果
        /// </summary>
        /// <param name="responseContentType">响应的ContentType</param>
        /// <returns></returns>
        protected override bool IsMatchAcceptContentType(MediaTypeHeaderValue responseContentType)
        {
            var mediaType = responseContentType.MediaType;
            return mediaType != null && allowMediaTypes.Contains(mediaType);
        }

        /// <summary>
        /// 设置强类型模型结果值
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
        public override async Task SetResultAsync(ApiResponseContext context)
        {
            var resultType = context.ActionDescriptor.Return.DataType.Type;
            context.Result = await context.XmlDeserializeAsync(resultType).ConfigureAwait(false);
        }
    }
}
