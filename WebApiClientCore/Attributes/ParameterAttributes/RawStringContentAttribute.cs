using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示将参数的文本内容作为请求内容
    /// </summary>
    public class RawStringContentAttribute : HttpContentAttribute, ICharSetable
    {
        /// <summary>
        /// 媒体类型
        /// </summary>
        private readonly string mediaType;

        /// <summary>
        /// 编码方式
        /// </summary>
        private Encoding encoding = Encoding.UTF8;

        /// <summary>
        /// 获取或设置编码名称
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public string CharSet
        {
            get => this.encoding.WebName;
            set => this.encoding = Encoding.GetEncoding(value);
        }

        /// <summary>
        /// 将参数的文本内容作为请求内容
        /// </summary>
        /// <param name="mediaType">媒体类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        public RawStringContentAttribute(string mediaType)
        {
            if (string.IsNullOrEmpty(mediaType))
            {
                throw new ArgumentNullException(nameof(mediaType));
            }
            this.mediaType = mediaType;
        }


        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected override Task SetHttpContentAsync(ApiParameterContext context)
        {
            var content = context.ParameterValue?.ToString() ?? string.Empty;
            context.HttpContext.RequestMessage.Content = new StringContent(content, this.encoding, this.mediaType);
            return Task.CompletedTask;
        }
    }
}
