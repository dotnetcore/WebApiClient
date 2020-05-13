using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示form-data表单
    /// </summary>
    class FormDataContent : MultipartContent, ICustomTracable
    {
        /// <summary>
        /// 分隔符
        /// </summary>
        private readonly string boundary;

        /// <summary>
        /// 省略文件内容的MultipartContent    
        /// </summary>
        private readonly Lazy<MultipartContent> ellipsisFileMultipartContent;

        /// <summary>
        /// 获取对应的ContentType
        /// </summary>
        public static string MediaType => "multipart/form-data";

        /// <summary>
        /// form-data表单
        /// </summary>
        public FormDataContent()
            : this(Guid.NewGuid().ToString())
        {
        }

        /// <summary>
        /// form-data表单
        /// </summary>
        /// <param name="boundary">分隔符</param>
        public FormDataContent(string boundary)
            : base("form-data", boundary)
        {
            this.boundary = boundary;
            this.ellipsisFileMultipartContent = new Lazy<MultipartContent>(this.CreateEllipsisFileMultipartContent, true);

            var parameter = new NameValueHeaderValue("boundary", boundary);
            this.Headers.ContentType.Parameters.Clear();
            this.Headers.ContentType.Parameters.Add(parameter);
        }

        /// <summary>
        /// 读取为文本信息
        /// </summary>
        /// <returns></returns>
        Task<string> ICustomTracable.ReadAsStringAsync()
        {
            return this.ellipsisFileMultipartContent.Value.ReadAsStringAsync();
        }

        /// <summary>
        /// 创建省略文件内容的MultipartContent
        /// </summary>
        /// <returns></returns>
        private MultipartContent CreateEllipsisFileMultipartContent()
        {
            var multipartContent = new FormDataContent(this.boundary);
            foreach (var httpContent in this)
            {
                if (httpContent is FormDataFileContent fileContent)
                {
                    multipartContent.Add(fileContent.ToEllipsisContent());
                }
                else
                {
                    multipartContent.Add(httpContent);
                }
            }
            return multipartContent;
        }
    }
}
