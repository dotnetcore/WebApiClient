using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 自动适应返回类型的处理
    /// 支持返回TaskOf(HttpResponseMessage)或TaskOf(byte[])或TaskOf(string)或TaskOf(Stream)
    /// 支持返回xml或json转换对应类型
    /// 没有任何IApiReturnAttribute特性修饰的接口方法，将默认为AutoReturn修饰
    /// </summary> 
    public class AutoReturnAttribute : ApiReturnAttribute
    {
        /// <summary>
        /// xml解析
        /// </summary>
        private static readonly IApiReturnAttribute xmlReturn = new XmlReturnAttribute();

        /// <summary>
        /// json解析
        /// </summary>
        private static readonly IApiReturnAttribute jsonReturn = new JsonReturnAttribute();

        /// <summary>
        /// 获取异步结果
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected override async Task<object> GetTaskResult(ApiActionContext context)
        {
            var response = context.ResponseMessage;
            var dataType = context.ApiActionDescriptor.Return.DataType;

            if (dataType == typeof(HttpResponseMessage))
            {
                return response;
            }

            if (dataType == typeof(string))
            {
                return await response.Content.ReadAsStringAsync();
            }

            if (dataType == typeof(byte[]))
            {
                return await response.Content.ReadAsByteArrayAsync();
            }

            if (dataType == typeof(Stream))
            {
                return await response.Content.ReadAsStreamAsync();
            }

            var contentType = new ContentType(response.Content.Headers.ContentType);
            if (contentType.IsApplicationJson() == true)
            {
                return await jsonReturn.GetTaskResult(context);
            }
            else if (contentType.IsApplicationXml() == true)
            {
                return await xmlReturn.GetTaskResult(context);
            }

            throw new ApiReturnNotSupportedExteption(response, dataType);
        }
    }
}
