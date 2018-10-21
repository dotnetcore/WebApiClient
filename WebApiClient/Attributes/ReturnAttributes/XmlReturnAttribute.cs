using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示使用XmlFormatter反序列化回复内容作为返回值
    /// </summary>
    public class XmlReturnAttribute : ApiReturnAttribute
    {
        /// <summary>
        /// 配置请求头的accept
        /// </summary>
        /// <param name="accept">请求头的accept</param>
        /// <returns></returns>
        protected override void ConfigureAccept(HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> accept)
        {
            accept.Add(new MediaTypeWithQualityHeaderValue(XmlContent.MediaType));
        }

        /// <summary>
        /// 获取异步结果
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected override async Task<object> GetTaskResult(ApiActionContext context)
        {
            var response = context.ResponseMessage;
            var xml = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var dataType = context.ApiActionDescriptor.Return.DataType.Type;
            var result = context.HttpApiConfig.XmlFormatter.Deserialize(xml, dataType);
            return result;
        }
    }
}
