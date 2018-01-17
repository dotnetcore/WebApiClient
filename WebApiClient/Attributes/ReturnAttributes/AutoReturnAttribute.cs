using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApiClient.Contexts;
using WebApiClient.Interfaces;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 自动适应返回类型的处理
    /// 支持返回TaskOf(HttpResponseMessage)或TaskOf(byte[])或TaskOf(string)或TaskOf(Stream)
    /// 支持返回xml或json转换对应类型
    /// 没有任何IApiReturnAttribute特性修饰的接口方法，将默认为AutoReturn修饰
    /// </summary> 
    public sealed class AutoReturnAttribute : ApiReturnAttribute
    {
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

            if (dataType == typeof(byte[]))
            {
                return await response.Content.ReadAsByteArrayAsync();
            }

            if (dataType == typeof(string))
            {
                return await response.Content.ReadAsStringAsync();
            }

            if (dataType == typeof(Stream))
            {
                return await response.Content.ReadAsStreamAsync();
            }

            var contentType = new ContentType(response.Content.Headers.ContentType);
            if (contentType.IsApplicationJson())
            {
                var jsonReturn = new JsonReturnAttribute() as IApiReturnAttribute;
                return await jsonReturn.GetTaskResult(context);
            }

            if (contentType.IsApplicationXml())
            {
                var xmlReturn = new XmlReturnAttribute() as IApiReturnAttribute;
                return await xmlReturn.GetTaskResult(context);
            }

            var message = string.Format("请求的回复内容不支持自动映射为类型{0}，请为接口设置合适的ApiReturnAttribute", dataType);
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
