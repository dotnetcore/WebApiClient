using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore.HttpContents;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 使用JsonSerializer序列化参数值得到的 json 文本作为 application/json 请求
    /// 每个Api只能注明于其中的一个参数
    /// </summary>
    public class JsonContentAttribute : HttpContentAttribute, ICharSetable
    {
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
        /// 设置参数到 http 请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
        [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "<Pending>")]
        protected override Task SetHttpContentAsync(ApiParameterContext context)
        {
            context.HttpContext.RequestMessage.Content = this.CreateContent(context);
            return Task.CompletedTask;
        }

        [RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
        [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
#if NET5_0_OR_GREATER
        private System.Net.Http.Json.JsonContent CreateContent(ApiParameterContext context)
        {
            var value = context.ParameterValue;
            var options = context.HttpContext.HttpApiOptions.JsonSerializeOptions;
            var valueType = value == null ? context.Parameter.ParameterType : value.GetType();
            var mediaType = Encoding.UTF8.Equals(this.encoding) ? defaultMediaType : new MediaTypeHeaderValue(JsonContent.MediaType) { CharSet = this.CharSet };
            return System.Net.Http.Json.JsonContent.Create(value, valueType, mediaType, options);
        }
        private static readonly MediaTypeHeaderValue defaultMediaType = new(JsonContent.MediaType);
#else
        private JsonContent CreateContent(ApiParameterContext context)
        {
            var value = context.ParameterValue;
            var options = context.HttpContext.HttpApiOptions.JsonSerializeOptions;
            return new JsonContent(value, options, this.encoding);
        }
#endif
    }
}
