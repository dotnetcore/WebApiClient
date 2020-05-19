using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WebApiClientCore.Parameters;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示参数内容为FileInfo类型的处理特性
    /// </summary>
    class FileInfoTypeAttribute : ApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override async Task OnRequestAsync(ApiParameterContext context)
        {
            if (context.ParameterValue is FileInfo fileInfo)
            {
                await AddFileAsync(context, fileInfo).ConfigureAwait(false);
            }
            else if (context.ParameterValue is IEnumerable<FileInfo> fileInfos)
            {
                foreach (var file in fileInfos)
                {
                    await AddFileAsync(context, file).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="context"></param>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        private static Task AddFileAsync(ApiParameterContext context, FileInfo fileInfo)
        {
            var formDataFile = new FormDataFile(fileInfo);
            return formDataFile.OnRequestAsync(context);
        }
    }
}
