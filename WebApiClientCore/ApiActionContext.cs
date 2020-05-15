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
        /// 获取调用Api得到的结果
        /// </summary>
        public object Result { get; internal set; }

        /// <summary>
        /// 获取调用Api产生的异常
        /// </summary>
        public Exception Exception { get; internal set; }


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
