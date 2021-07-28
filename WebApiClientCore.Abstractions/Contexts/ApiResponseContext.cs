using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

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
        /// 获取结果状态
        /// </summary>
        public ResultStatus ResultStatus { get; private set; }

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
        /// Api响应的上下文
        /// </summary>
        /// <param name="context">请求上下文</param>
        public ApiResponseContext(ApiRequestContext context)
            : base(context.HttpContext, context.ActionDescriptor, context.Arguments, context.Properties, false)
        {
        }
    }
}
