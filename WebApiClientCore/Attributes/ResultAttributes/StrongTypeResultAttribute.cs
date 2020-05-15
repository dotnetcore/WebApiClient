using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示强类型模型结果抽象特性
    /// </summary>
    public abstract class StrongTypeResultAttribute : ApiResultAttribute
    {
        /// <summary>
        /// 媒体类型
        /// </summary>
        private readonly MediaTypeWithQualityHeaderValue mediaType;

        /// <summary>
        /// 获取或设置是否必须匹配ContentType
        /// </summary>
        public bool EnsureMatchContentType { get; set; } = true;

        /// <summary>
        /// 强类型模型结果抽象特性
        /// </summary>
        /// <param name="accpetContentType">收受的内容类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        public StrongTypeResultAttribute(string accpetContentType)
        {
            this.mediaType = MediaTypeWithQualityHeaderValue.Parse(accpetContentType);
        }

        /// <summary>
        /// 配置请求头的accept
        /// </summary>
        /// <param name="accept">请求头的accept</param>
        public sealed override void ConfigureAccept(HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> accept)
        {
            accept.Add(this.mediaType);
        }

        /// <summary>
        /// 设置结果值
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public sealed override async Task SetResultAsync(ApiActionContext context)
        {
            if (context.ApiAction.Return.DataType.IsStrongType == false)
            {
                return;
            }

            if (this.EnsureMatchContentType == true)
            {
                var contenType = new ContentType(context.HttpContext.ResponseMessage.Content.Headers.ContentType);
                if (contenType.IsJson() == false)
                {
                    return;
                }
            }

            await this.SetStrongTypeResultAsync(context);
        }

        /// <summary>
        /// 设置强类型模型结果值
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract Task SetStrongTypeResultAsync(ApiActionContext context);
    }
}
