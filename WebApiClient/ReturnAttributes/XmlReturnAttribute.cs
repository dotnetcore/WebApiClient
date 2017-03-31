using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WebApiClient
{
    /// <summary>
    /// 表示将回复的Xml结果作反序化为指定类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class XmlReturnAttribute : ApiReturnAttribute
    {
        /// <summary>
        /// 获取异步结果
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override async Task<object> GetTaskResult(ApiActionContext context)
        {
            var response = context.ResponseMessage;
            var dataType = context.ApiActionDescriptor.ReturnDataType;
            var xmlSerializer = new XmlSerializer(dataType);

            using (var stream = new MemoryStream())
            {
                await response.Content.CopyToAsync(stream);
                stream.Position = 0;
                return xmlSerializer.Deserialize(stream);
            }
        }
    }
}
