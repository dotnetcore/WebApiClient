using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示Http失败状态码异常
    /// </summary>
    public class HttpStatusFailureException : HttpRequestException
    {
        /// <summary>
        /// 获取Http接口的配置项
        /// </summary>
        public HttpApiConfig HttpApiConfig { get; private set; }

        /// <summary>
        /// 获取响应消息
        /// </summary>
        public HttpResponseMessage ResponseMessage { get; private set; }

        /// <summary>
        /// 获取响应状态码
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get => this.ResponseMessage.StatusCode;
        }

        /// <summary>
        /// 返回异常提示
        /// </summary>
        public override string Message
        {
            get
            {
                var code = (int)this.ResponseMessage.StatusCode;
                var reason = this.ResponseMessage.ReasonPhrase;
                return $"服务器响应了错误的的http状态码：{code} {reason}";
            }
        }

        /// <summary>
        /// Http失败状态码异常
        /// </summary>
        /// <param name="httpApiConfig">Http接口的配置项</param>
        /// <param name="responseMessage">响应消息</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpStatusFailureException(HttpApiConfig httpApiConfig, HttpResponseMessage responseMessage)
        {
            this.HttpApiConfig = httpApiConfig ?? throw new ArgumentNullException(nameof(httpApiConfig));
            this.ResponseMessage = responseMessage ?? throw new ArgumentNullException(nameof(responseMessage));
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
            var content = this.ResponseMessage.Content;
            var contentType = new ContentType(content.Headers.ContentType);

            if (contentType.IsApplicationJson() == true)
            {
                var json = await content.ReadAsStringAsync().ConfigureAwait(false);
                return (TResult)this.HttpApiConfig.JsonFormatter.Deserialize(json, dataType);
            }
            else if (contentType.IsApplicationXml() == true)
            {
                var xml = await content.ReadAsStringAsync().ConfigureAwait(false);
                return (TResult)this.HttpApiConfig.XmlFormatter.Deserialize(xml, dataType);
            }
            throw new ApiReturnNotSupportedExteption(this.ResponseMessage, dataType);
        }
    }
}
