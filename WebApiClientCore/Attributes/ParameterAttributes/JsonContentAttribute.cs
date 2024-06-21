using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 使用JsonSerializer序列化参数值得到的 json 文本作为 application/json 请求
    /// 每个Api只能注明于其中的一个参数
    /// </summary>
    public class JsonContentAttribute : HttpContentAttribute, ICharSetable, IChunkedable
    {
        private const string jsonMediaType = "application/json";
        private static readonly MediaTypeHeaderValue defaultMediaType = new(jsonMediaType);

        private MediaTypeHeaderValue mediaType = defaultMediaType;
        private Encoding encoding = Encoding.UTF8;

        /// <summary>
        /// 获取或设置是否允许 chunked 传输
        /// 默认为 true
        /// </summary>
        public bool AllowChunked { get; set; } = true;

        /// <summary>
        /// 获取或设置编码名称
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public string CharSet
        {
            get => this.encoding.WebName;
            set
            {
                this.encoding = Encoding.GetEncoding(value);
                this.mediaType = new MediaTypeHeaderValue(jsonMediaType) { CharSet = this.encoding.WebName };
            }
        }

        /// <summary>
        /// 设置参数到 http 请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
        [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "<Pending>")]
        protected override Task SetHttpContentAsync(ApiParameterContext context)
        {
            var value = context.ParameterValue;
            var options = context.HttpContext.HttpApiOptions.JsonSerializeOptions;

            if (this.AllowChunked)
            {
                var valueType = value == null ? context.Parameter.ParameterType : value.GetType();
                context.HttpContext.RequestMessage.Content = JsonContent.Create(value, valueType, this.mediaType, options);
            }
            else
            {
                context.HttpContext.RequestMessage.Content = new HttpContents.JsonContent(value, options, this.encoding);
            }
            return Task.CompletedTask;
        }
    }
}
