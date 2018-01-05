using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示将回复的Json结果作反序化为指定类型
    /// 依赖于HttpApiConfig.JsonFormatter
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
            var json = await response.Content.ReadAsStringAsync();

            var dataType = context.ApiActionDescriptor.Return.DataType;
            var result = context.HttpApiConfig.JsonFormatter.Deserialize(json, dataType);
            return result;
        }
    }
}
