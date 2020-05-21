using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示Api响应的上下文
    /// </summary>
    public class ApiResponseContext : ApiRequestContext
    {
        private object? result;
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
                this.exception = null;
                this.ResultStatus = ResultStatus.HasResult;
            }
        }

        /// <summary>
        /// 获取或设置异常值
        /// 在IApiReturnAttribute设置该值之后会中断下一个IApiReturnAttribute的执行
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public Exception? Exception
        {
            get => this.exception;
            set
            {
                this.result = null;
                this.exception = value ?? throw new ArgumentNullException(nameof(Exception));
                this.ResultStatus = ResultStatus.HasException;
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
            : base(context.HttpContext, context.ApiAction, context.Arguments, context.UserDatas, context.CancellationTokens)
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

            var formatter = this.HttpContext.Services.GetRequiredService<IJsonFormatter>();
            var json = await this.HttpContext.ResponseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var options = this.HttpContext.Options.JsonDeserializeOptions;
            return formatter.Deserialize(json, objType, options);
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

            var formatter = this.HttpContext.Services.GetRequiredService<IXmlFormatter>();
            var xml = await this.HttpContext.ResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            return formatter.Deserialize(xml, objType);
        }
    }
}
