using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore.HttpContents;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 使用JsonSerializer序列化参数值得到的json文本作为application/json请求
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
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
        [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "<Pending>")]
        protected override Task SetHttpContentAsync(ApiParameterContext context)
        {
            var options = context.HttpContext.HttpApiOptions.JsonSerializeOptions;
            context.HttpContext.RequestMessage.Content = new JsonContent(context.ParameterValue, options, this.encoding);
            return Task.CompletedTask;
        }
    }
}
