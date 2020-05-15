using System;
using System.Collections.Generic;
using System.Threading;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示Api请求的上下文
    /// </summary>
    public class ApiRequestContext
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
        public object[] Arguments { get; }


        /// <summary>
        /// 获取自定义数据的存储和访问容器
        /// </summary>
        public Tags Tags { get; }

        /// <summary>
        /// 获取请求取消令牌集合
        /// </summary>
        public IList<CancellationToken> CancellationTokens { get; }


        /// <summary>
        /// 请求Api的上下文
        /// </summary> 
        /// <param name="httpContext"></param> 
        /// <param name="apiAction"></param>
        /// <param name="arguments"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiRequestContext(HttpContext httpContext, ApiActionDescriptor apiAction, object[] arguments)
            : this(httpContext, apiAction, arguments, new Tags(), new List<CancellationToken>())
        {
        }

        /// <summary>
        /// 请求Api的上下文
        /// </summary> 
        /// <param name="httpContext"></param> 
        /// <param name="apiAction"></param>
        /// <param name="arguments"></param>
        /// <param name="tags"></param>
        /// <param name="cancellationTokens"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected ApiRequestContext(HttpContext httpContext, ApiActionDescriptor apiAction, object[] arguments, Tags tags, IList<CancellationToken> cancellationTokens)
        {
            this.HttpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
            this.ApiAction = apiAction ?? throw new ArgumentNullException(nameof(apiAction));
            this.Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));

            this.Tags = tags;
            this.CancellationTokens = cancellationTokens;
        }
    }
}
