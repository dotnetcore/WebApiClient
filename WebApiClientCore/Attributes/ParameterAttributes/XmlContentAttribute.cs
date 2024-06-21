using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore.HttpContents;
using WebApiClientCore.Internals;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 使用XmlSerializer序列化参数值得到的 xml 文本作为 application/xml 请求
    /// </summary>
    public class XmlContentAttribute : HttpContentAttribute, ICharSetable
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
        protected override Task SetHttpContentAsync(ApiParameterContext context)
        {
            using var bufferWriter = new RecyclableBufferWriter<char>();
            context.SerializeToXml(this.encoding, bufferWriter);
            context.HttpContext.RequestMessage.Content = new XmlContent(bufferWriter.WrittenSpan, this.encoding);
            return Task.CompletedTask;
        }
    }
}
