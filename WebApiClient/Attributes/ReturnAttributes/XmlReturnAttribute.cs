using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 使用XmlFormatter反序列化回复内容作为返回值
    /// </summary>
    public class XmlReturnAttribute :  ApiReturnAttribute 
    {
        /// <summary>
        /// 获取异步结果
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected override async Task<object> GetTaskResult(ApiActionContext context)
        {
            var response = context.ResponseMessage;
            var xml = await response.Content.ReadAsStringAsync();

            var dataType = context.ApiActionDescriptor.Return.DataType;
            var result = context.HttpApiConfig.XmlFormatter.Deserialize(xml, dataType);
            return result;
        }
    }
}
