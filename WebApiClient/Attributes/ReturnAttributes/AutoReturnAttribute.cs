using System.Net.Http.Headers;
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
        /// 配置请求头的accept
        /// </summary>
        /// <param name="accept">请求头的accept</param>
        /// <returns></returns>
        protected override void ConfigureAccept(HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> accept)
        {
            accept.Add(new MediaTypeWithQualityHeaderValue(JsonContent.MediaType, 0.9d));
            accept.Add(new MediaTypeWithQualityHeaderValue(XmlContent.MediaType, 0.8d));
            accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.1d));
        }

        /// <summary>
        /// 获取异步结果
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected override async Task<object> GetTaskResult(ApiActionContext context)
        {
            var response = context.ResponseMessage;
            var dataType = context.ApiActionDescriptor.Return.DataType;

            if (dataType.IsHttpResponseMessage == true)
            {
                return response;
            }

            if (dataType.IsString == true)
            {
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }

            if (dataType.IsByteArray == true)
            {
                return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            }

            if (dataType.IsStream == true)
            {
                return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            }

            if (dataType.IsHttpResponseWrapper == true)
            {
                return dataType.HttpResponseWrapperFactory.Invoke(response);
            }

            var contentType = new ContentType(response.Content.Headers.ContentType);
            if (contentType.IsApplicationJson() == true)
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return context.HttpApiConfig.JsonFormatter.Deserialize(json, dataType.Type);
            }

            if (contentType.IsApplicationXml() == true)
            {
                var xml = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return context.HttpApiConfig.XmlFormatter.Deserialize(xml, dataType.Type);
            }

            throw new ApiReturnNotSupportedExteption(response, dataType.Type);
        }
    }
}
