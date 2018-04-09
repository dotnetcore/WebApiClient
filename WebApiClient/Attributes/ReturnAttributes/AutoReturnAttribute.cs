using System;
using System.IO;
using System.Net.Http;
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
            var dataType = context.ApiActionDescriptor.Return.DataType;

            if (dataType == typeof(HttpResponseMessage))
            {
                return response;
            }

            if (dataType == typeof(string))
            {
                return await response.Content.ReadAsStringAsync();
            }

            if (dataType == typeof(byte[]))
            {
                return await response.Content.ReadAsByteArrayAsync();
            }

            if (dataType == typeof(Stream))
            {
                return await response.Content.ReadAsStreamAsync();
            }

            var contentType = new ContentType(response.Content.Headers.ContentType);
            if (contentType.IsApplicationJson() == true)
            {
                return await jsonReturn.GetTaskResult(context);
            }
            else if (contentType.IsApplicationXml() == true)
            {
                return await xmlReturn.GetTaskResult(context);
            }

            var message = $"请求的回复内容不支持自动映射为类型{dataType}，请为接口设置合适的ApiReturnAttribute";
            throw new NotSupportedException(message);
        }

        /// <summary>
        /// 表示回复的ContentType
        /// </summary>
        private struct ContentType
        {
            /// <summary>
            /// ContentType内容
            /// </summary>
            private readonly string contentType;

            /// <summary>
            /// 回复的ContentType
            /// </summary>
            /// <param name="contentType">ContentType内容</param>
            public ContentType(MediaTypeHeaderValue contentType)
            {
                this.contentType = contentType?.MediaType;
            }

            /// <summary>
            /// 是否为某个Mime
            /// </summary>
            /// <param name="mediaType"></param>
            /// <returns></returns>
            public bool Is(string mediaType)
            {
                return this.contentType != null && this.contentType.StartsWith(mediaType, StringComparison.OrdinalIgnoreCase);
            }

            /// <summary>
            /// 是否为json
            /// </summary>
            /// <returns></returns>
            public bool IsApplicationJson()
            {
                return this.Is("application/json") || this.Is("text/json");
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
