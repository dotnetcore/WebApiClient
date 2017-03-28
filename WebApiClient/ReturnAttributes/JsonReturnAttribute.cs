using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    /// <summary>
    /// 表示将回复的Json结果作反序化为指定类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class JsonReturnAttribute : ApiReturnAttribute
    {
        /// <summary>
        /// 异步获取结果
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override async Task<object> GetResultAsync(ApiActionContext context)
        {
            var response = await context.ResponseMessage;
            var json = await response.Content.ReadAsStringAsync();

            var dataType = context.ApiActionDescriptor.ReturnDataType;
            var result = context.HttpApiClient.JsonFormatter.Deserialize(json, dataType);
            return result;
        }
    }
}
