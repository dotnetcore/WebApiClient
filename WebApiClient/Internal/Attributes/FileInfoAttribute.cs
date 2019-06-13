using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
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
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        public Task BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var fileInfo = parameter.Value as FileInfo;
            if (fileInfo != null)
            {
                var stream = fileInfo.Open(FileMode.Open, FileAccess.Read);
                var fileName = Path.GetFileName(fileInfo.FullName);
                var encodedFileName = HttpUtility.UrlEncode(fileName, Encoding.UTF8);
                context.RequestMessage.AddMulitpartFile(stream, parameter.Name, encodedFileName, null);
            }
            return ApiTask.CompletedTask;
        }
    }
}
