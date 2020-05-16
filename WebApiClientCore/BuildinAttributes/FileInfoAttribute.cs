using System.IO;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;
using WebApiClientCore.Parameters;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示参数内容为FileInfo处理特性
    /// </summary>
    class FileInfoAttribute : ApiParameterAttribute
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
                var formDataFile = new FormDataFile(fileInfo);
                await formDataFile.OnRequestAsync(context);
            }
        }
    }
}
