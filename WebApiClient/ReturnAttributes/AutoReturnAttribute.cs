using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 自动适应返回类型处理
    /// 支持返回TaskOf(HttpResponseMessage)或TaskOf(byte[])或TaskOf(string)
    /// 支持返回xml或json转换对应类型
    /// 此特性不需要显示声明
    /// </summary> 
    public class AutoReturnAttribute : Attribute, IApiReturnAttribute
    {
        /// <summary>
        /// 获取异步结果
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public virtual async Task<object> GetTaskResult(ApiActionContext context)
        {
            var response = context.ResponseMessage;
            var returnType = context.ApiActionDescriptor.ReturnDataType;

            if (returnType == typeof(HttpResponseMessage))
            {
                return response;
            }

            if (returnType == typeof(byte[]))
            {
                return await response.Content.ReadAsByteArrayAsync();
            }

            if (returnType == typeof(string))
            {
                return await response.Content.ReadAsStringAsync();
            }

            var contentType = new ContentType(response.Content.Headers.ContentType);
            if (contentType.IsApplicationJson())
            {
                return await new JsonReturnAttribute().GetTaskResult(context);
            }
            if (contentType.IsApplicationXml())
            {
                return await new XmlReturnAttribute().GetTaskResult(context);
            }

            var message = string.Format("不支持的类型{0}的解析", returnType);
            throw new NotSupportedException(message);
        }


        /// <summary>
        /// 表示回复的ContentType
        /// </summary>
        class ContentType
        {
            /// <summary>
            /// ContentType内容
            /// </summary>
            private readonly string contenType;

            /// <summary>
            /// 回复的ContentType
            /// </summary>
            /// <param name="contenType">ContentType内容</param>
            public ContentType(MediaTypeHeaderValue contenType)
            {
                this.contenType = contenType == null ? null : contenType.MediaType;
            }

            /// <summary>
            /// 是否为某个Mime
            /// </summary>
            /// <param name="mime"></param>
            /// <returns></returns>
            private bool Is(string mime)
            {
                return this.contenType != null && this.contenType.StartsWith(mime, StringComparison.OrdinalIgnoreCase);
            }

            /// <summary>
            /// 是否为json
            /// </summary>
            /// <returns></returns>
            public bool IsApplicationJson()
            {
                return this.Is("application/json");
            }

            /// <summary>
            /// 是否为xml
            /// </summary>
            /// <returns></returns>
            public bool IsApplicationXml()
            {
                return this.Is("application/xml") || this.Is("text/xml");
            }
        }
    }
}
