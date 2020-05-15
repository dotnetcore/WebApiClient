using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示参数内容为FileInfo处理特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    class FileInfoAttribute : Attribute, IApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public Task BeforeRequestAsync(ApiParameterContext context )
        {
            var fileInfo = context.ParameterValue as FileInfo;
            if (fileInfo != null)
            {
                var stream = fileInfo.Open(FileMode.Open, FileAccess.Read);
                var fileName = Path.GetFileName(fileInfo.FullName);
                var encodedFileName = HttpUtility.UrlEncode(fileName, Encoding.UTF8);
                context.HttpContext.RequestMessage.AddFormDataFile(stream, context.Parameter.Name, encodedFileName, null);
            }
            return Task.CompletedTask;
        }
    }
}
