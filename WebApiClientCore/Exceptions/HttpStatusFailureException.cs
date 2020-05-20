using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApiClientCore.Exceptions
{
    /// <summary>
    /// 表示Http失败状态码异常
    /// </summary>
    public class HttpStatusFailureException : HttpApiException
    {
        /// <summary>
        /// 上下文
        /// </summary>
        private readonly ApiResponseContext context;

        /// <summary>
        /// 获取响应消息
        /// </summary>
        public HttpResponseMessage ResponseMessage => this.context.HttpContext.ResponseMessage;

        /// <summary>
        /// 获取响应状态码
        /// </summary>
        public HttpStatusCode StatusCode => this.context.HttpContext.ResponseMessage.StatusCode;


        /// <summary>
        /// 返回异常提示
        /// </summary>
        public override string Message
        {
            get
            {
                var code = (int)this.ResponseMessage.StatusCode;
                var reason = this.ResponseMessage.ReasonPhrase;
                return Resx.failure_StatusCode.Format(code, reason);
            }
        }

        /// <summary>
        /// Http失败状态码异常
        /// </summary> 
        /// <param name="context"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpStatusFailureException(ApiResponseContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// 返回响应结果的String表述
        /// </summary>
        /// <returns></returns>
        public Task<string> ReadAsStringAsync()
        {
            return this.ResponseMessage.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// 返回响应结果的Stream表述
        /// </summary>
        /// <returns></returns>
        public Task<Stream> ReadAsStreamAsync()
        {
            return this.ResponseMessage.Content.ReadAsStreamAsync();
        }

        /// <summary>
        /// 返回响应结果的byte[]表述
        /// </summary>
        /// <returns></returns>
        public Task<byte[]> ReadAsByteArrayAsync()
        {
            return this.ResponseMessage.Content.ReadAsByteArrayAsync();
        }

        /// <summary>
        /// 根据ContentType自动选择json或xml将响应结果反序列化为TResult类型
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <exception cref="ApiReturnNotSupportedExteption"></exception>
        /// <returns></returns>
        public async Task<TResult> ReadAsAsync<TResult>()
        {
            var dataType = typeof(TResult);
            var contentType = new ContentType(this.ResponseMessage.Content.Headers.ContentType);

            if (contentType.IsJson() == true)
            {
                return (TResult)await this.context.JsonDeserializeAsync(dataType).ConfigureAwait(false);
            }
            else if (contentType.IsXml() == true)
            {
                return (TResult)await this.context.XmlDeserializeAsync(dataType).ConfigureAwait(false);
            }
            throw new ApiReturnNotSupportedExteption(this.ResponseMessage, dataType);
        }
    }
}
