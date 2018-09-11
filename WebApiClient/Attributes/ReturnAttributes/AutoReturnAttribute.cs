using System;
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
            var dataType = context.ApiActionDescriptor.Return.ReturnType.DataType;

            if (dataType.Type == typeof(HttpResponseMessage))
            {
                return response;
            }

            if (dataType.Type == typeof(string))
            {
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }

            if (dataType.Type == typeof(byte[]))
            {
                return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            }

            if (dataType.Type == typeof(Stream))
            {
                return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            }

            if (dataType.IsHttpResponseWrapper == true)
            {
                return Activator.CreateInstance(dataType.Type, response);
            }

            var contentType = new ContentType(response.Content.Headers.ContentType);
            if (contentType.IsApplicationJson() == true)
            {
                return await jsonReturn.GetTaskResult(context).ConfigureAwait(false);
            }
            else if (contentType.IsApplicationXml() == true)
            {
                return await xmlReturn.GetTaskResult(context).ConfigureAwait(false);
            }

            throw new ApiReturnNotSupportedExteption(response, dataType.Type);
        }
    }
}
