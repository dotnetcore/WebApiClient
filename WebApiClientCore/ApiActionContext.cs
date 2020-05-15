using System;
using System.Collections.Generic;
using System.Threading;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示请求Api的上下文
    /// </summary>
    public class ApiActionContext
    {
        private object result;
        private Exception exception;

        /// <summary>
        /// 获取http上下文
        /// </summary>
        public HttpContext HttpContext { get; }

        /// <summary>
        /// 获取关联的ApiAction描述
        /// </summary>
        public ApiActionDescriptor ApiAction { get; }

        /// <summary>
        /// 获取请求参数值
        /// </summary>
        public IList<object> Arguments { get; }


        /// <summary>
        /// 获取自定义数据的存储和访问容器
        /// </summary>
        public Tags Tags { get; } = new Tags();

        /// <summary>
        /// 获取请求取消令牌集合
        /// </summary>
        public IList<CancellationToken> CancellationTokens { get; } = new List<CancellationToken>();

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
            internal set
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
        /// 请求Api的上下文
        /// </summary> 
        /// <param name="httpContext"></param> 
        /// <param name="apiAction"></param>
        /// <param name="arguments"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiActionContext(HttpContext httpContext, ApiActionDescriptor apiAction, params object[] arguments)
        {
            this.HttpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
            this.ApiAction = apiAction ?? throw new ArgumentNullException(nameof(apiAction));
            this.Arguments = new List<object>(arguments);
        }
    }
}
