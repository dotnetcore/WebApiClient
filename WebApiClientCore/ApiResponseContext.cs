using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示Api响应的上下文
    /// </summary>
    public class ApiResponseContext : ApiRequestContext
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private object? result;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Exception? exception;

        /// <summary>
        /// 获取或设置结果值
        /// 在IApiReturnAttribute设置该值之后会中断下一个IApiReturnAttribute的执行
        /// </summary>
        public object? Result
        {
            get => this.result;
            set
            {
                this.result = value;
                this.ResultStatus = ResultStatus.HasResult;
                this.exception = null;
            }
        }

        /// <summary>
        /// 获取或设置异常值
        /// 在IApiReturnAttribute设置该值之后会中断下一个IApiReturnAttribute的执行
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        [DisallowNull]
        public Exception? Exception
        {
            get => this.exception;
            set
            {
                this.exception = value ?? throw new ArgumentNullException(nameof(Exception));
                this.ResultStatus = ResultStatus.HasException;
                this.result = null;
            }
        }

        /// <summary>
        /// 获取结果状态
        /// </summary>
        public ResultStatus ResultStatus { get; private set; }

        /// <summary>
        /// Api响应的上下文
        /// </summary>
        /// <param name="context">请求上下文</param>
        public ApiResponseContext(ApiRequestContext context)
            : base(context.HttpContext, context.ApiAction, context.Arguments, context.Properties)
        {
        }

        /// <summary>
        /// 使用Json反序列化响应内容为目标类型
        /// </summary>
        /// <param name="objType">目标类型</param>
        /// <returns></returns>
        public async Task<object?> JsonDeserializeAsync(Type objType)
        {
            if (this.HttpContext.ResponseMessage == null)
            {
                return objType.DefaultValue();
            }

            var content = this.HttpContext.ResponseMessage.Content;
            if (content == null)
            {
                return objType.DefaultValue();
            }

            var encoding = content.GetEncoding();
            var options = this.HttpContext.HttpApiOptions.JsonDeserializeOptions;
            var serializer = this.HttpContext.ServiceProvider.GetJsonSerializer();

            if (Encoding.UTF8.Equals(encoding) == false)
            {
                var byteArray = await content.ReadAsByteArrayAsync().ConfigureAwait(false);
                var utf8Json = Encoding.Convert(encoding, Encoding.UTF8, byteArray);
                return serializer.Deserialize(utf8Json, objType, options);
            }

            if (content.IsBuffered() == false)
            {
                var utf8Json = await content.ReadAsStreamAsync().ConfigureAwait(false);
                return await serializer.DeserializeAsync(utf8Json, objType, options).ConfigureAwait(false);
            }
            else
            {
                var utf8Json = await content.ReadAsByteArrayAsync().ConfigureAwait(false);
                return serializer.Deserialize(utf8Json, objType, options);
            }
        }

        /// <summary>
        /// 使用Xml反序列化响应内容为目标类型
        /// </summary>
        /// <param name="objType">目标类型</param>
        /// <returns></returns>
        public async Task<object?> XmlDeserializeAsync(Type objType)
        {
            if (this.HttpContext.ResponseMessage == null)
            {
                return objType.DefaultValue();
            }

            var options = this.HttpContext.HttpApiOptions.XmlDeserializeOptions;
            var serializer = this.HttpContext.ServiceProvider.GetXmlSerializer();
            var xml = await this.HttpContext.ResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            return serializer.Deserialize(xml, objType, options);
        }
    }
}
