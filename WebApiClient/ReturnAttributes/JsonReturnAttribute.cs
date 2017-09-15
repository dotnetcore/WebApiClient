using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示将回复的Json结果作反序化为指定类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class JsonReturnAttribute : Attribute, IApiReturnAttribute
    {
        /// <summary>
        /// 获取异步结果
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public virtual async Task<object> GetTaskResult(ApiActionContext context)
        {
            var response = context.ResponseMessage.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var dataType = context.ApiActionDescriptor.ReturnDataType;
            var result = context.HttpApiClientConfig.JsonFormatter.Deserialize(json, dataType);
            return result;
        }
    }
}
