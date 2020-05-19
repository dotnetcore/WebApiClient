using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示强类型模型结果抽象特性
    /// </summary>
    public abstract class ModelTypeResultAttribute : ApiResultAttribute
    {
        /// <summary>
        /// 获取接受的媒体类型
        /// </summary>
        protected MediaTypeWithQualityHeaderValue AcceptContentType { get; }

        /// <summary>
        /// 获取或设置是否确保响应的ContentType与指定可接受的ContentType一致
        /// </summary>
        public bool EnsureMatchAcceptContentType { get; set; } = true;

        /// <summary>
        /// 强类型模型结果抽象特性
        /// </summary>
        /// <param name="accpetContentType">收受的内容类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ModelTypeResultAttribute(MediaTypeWithQualityHeaderValue accpetContentType)
        {
            this.AcceptContentType = accpetContentType ?? throw new ArgumentNullException(nameof(accpetContentType));
        }

        /// <summary>
        /// 配置请求头的accept
        /// </summary>
        /// <param name="accept">请求头的accept</param>
        public sealed override void ConfigureAccept(HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> accept)
        {
            accept.Add(this.AcceptContentType);
        }

        /// <summary>
        /// 设置结果值
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public sealed override async Task SetResultAsync(ApiResponseContext context)
        {
            if (context.ApiAction.Return.DataType.IsModelType == false)
            {
                return;
            }

            if (this.EnsureMatchAcceptContentType == true)
            {
                var contenType = context.HttpContext.ResponseMessage.Content.Headers.ContentType;
                if (this.IsMatchAcceptContentType(contenType) == false)
                {
                    return;
                }
            }

            await this.SetModelTypeResultAsync(context).ConfigureAwait(false);
        }

        /// <summary>
        /// 验证响应的ContentType与AcceptContentType是否匹配
        /// </summary>
        /// <param name="responseContentType"></param>
        /// <returns></returns>
        protected virtual bool IsMatchAcceptContentType(MediaTypeHeaderValue responseContentType)
        {
            var mediaType = responseContentType?.MediaType;
            return this.AcceptContentType.MediaType.StartsWith(mediaType, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 设置强类型模型结果值
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract Task SetModelTypeResultAsync(ApiResponseContext context);
    }
}
