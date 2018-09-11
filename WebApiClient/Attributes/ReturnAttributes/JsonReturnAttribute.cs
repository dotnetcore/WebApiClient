using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 使用JsonFormatter反序列化回复内容作为返回值
    /// </summary>
    public class JsonReturnAttribute : ApiReturnAttribute
    {
        /// <summary>
        /// 获取异步结果
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected override async Task<object> GetTaskResult(ApiActionContext context)
        {
            var response = context.ResponseMessage;
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var dataType = context.ApiActionDescriptor.Return.ReturnType.DataType.Type;
            var result = context.HttpApiConfig.JsonFormatter.Deserialize(json, dataType);
            return result;
        }
    }
}
