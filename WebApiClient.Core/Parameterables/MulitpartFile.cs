using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Parameterables
{
    /// <summary>
    /// 表示将自身作为multipart/form-data的一个文件项
    /// </summary>
    [DebuggerDisplay("FileName = {FileName}")]
    public class MulitpartFile : IApiParameterable
    {
        /// <summary>
        /// 流
        /// </summary>
        private readonly Stream stream;

        /// <summary>
        /// 文件路径
        /// </summary>
        private readonly string filePath;

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; private set; }

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
        public MulitpartFile(byte[] buffer, string fileName) :
            this(new MemoryStream(buffer ?? throw new ArgumentNullException(nameof(buffer))), fileName)
        {
        }

        /// <summary>
        /// 将自身作为multipart/form-data的一个文件项
        /// </summary>
        /// <param name="stream">数据流</param>
        /// <param name="fileName">文件友好名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        public MulitpartFile(Stream stream, string fileName)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
            this.FileName = fileName;
        }

        /// <summary>
        /// multipart/form-data的一个文件项
        /// </summary>
        /// <param name="localFilePath">本地文件路径</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public MulitpartFile(string localFilePath)
        {
            if (string.IsNullOrEmpty(localFilePath))
            {
                throw new ArgumentNullException(nameof(localFilePath));
            }

            if (File.Exists(localFilePath) == false)
            {
                throw new FileNotFoundException(localFilePath);
            }

            this.filePath = localFilePath;
            this.FileName = Path.GetFileName(localFilePath);
        }

        /// <summary>
        /// 执行请求前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        async Task IApiParameterable.BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            context.RequestMessage.AddMulitpartFile(this.GetStream(), parameter.Name, this.FileName, this.ContentType);
            await ApiTask.CompletedTask;
        }

        /// <summary>
        /// 获取文件流
        /// </summary>
        /// <returns></returns>
        private Stream GetStream()
        {
            if (this.stream != null)
            {
                return this.stream;
            }
            else
            {
                return new FileStream(this.filePath, FileMode.Open, FileAccess.Read);
            }
        }
    }
}
