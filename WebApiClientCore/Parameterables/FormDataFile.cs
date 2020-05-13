using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebApiClientCore.Parameterables
{
    /// <summary>
    /// 表示将自身作为multipart/form-data的一个文件项
    /// </summary>
    [DebuggerDisplay("FileName = {FileName}")]
    public class FormDataFile : IApiParameterable
    {
        /// <summary>
        /// 获取或设置包装的内部数据流
        /// </summary>
        private readonly Stream stream;

        /// <summary>
        /// 获取文件好友名称
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// 获取编码后的文件好友名称
        /// </summary>
        public virtual string EncodedFileName
        {
            get => HttpUtility.UrlEncode(this.FileName, Encoding.UTF8);
        }

        /// <summary>
        /// 获取或设置文件的Mime
        /// </summary>
        public string ContentType { get; set; } = "application/octet-stream";

        /// <summary>
        /// 将自身作为multipart/form-data的一个文件项
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <param name="fileName">文件友好名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        public FormDataFile(byte[] buffer, string fileName) :
            this(new MemoryStream(buffer ?? throw new ArgumentNullException(nameof(buffer))), fileName)
        {
        }

        /// <summary>
        /// 将自身作为multipart/form-data的一个文件项
        /// </summary>
        /// <param name="stream">数据流</param>
        /// <param name="fileName">文件友好名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        public FormDataFile(Stream stream, string fileName)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
            this.FileName = fileName;
        }

        /// <summary>
        /// 执行请求前
        /// </summary>
        /// <param name="context">上下文</param>
        async Task IApiParameterable.BeforeRequestAsync(ApiParameterContext context)
        {
            await this.BeforeRequestAsync(context).ConfigureAwait(false);
        }

        /// <summary>
        /// 执行请求前
        /// </summary>
        /// <param name="context">上下文</param>
        protected virtual async Task BeforeRequestAsync(ApiParameterContext context)
        {
            context.RequestMessage.AddFormDataFile(this.stream, context.Parameter.Name, this.EncodedFileName, this.ContentType);
            await Task.CompletedTask;
        }
    }
}
