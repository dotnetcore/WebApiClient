using System;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示Api响应的上下文
    /// </summary>
    public class ApiResponseContext : ApiRequestContext
    {
        private object result;
        private Exception exception;

        /// <summary>
        /// 获取或设置结果值
        /// </summary>
        public object Result
        {
            get => this.result;
            set
            {
                this.result = value;
                this.ResultStatus = ResultStatus.HasResult;
            }
        }

        /// <summary>
        /// 获取或设置异常值
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public Exception Exception
        {
            get => this.exception;
            set
            {
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
            : base(context.HttpContext, context.ApiAction, context.Arguments, context.Tags, context.CancellationTokens)
        {
        }
    }
}
