using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示参数内容为CancellationToken处理特性
    /// </summary>
    class CancellationTokenAttribute : Attribute, IApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="next"></param>
        /// <returns></returns>
        public Task BeforeRequestAsync(ApiParameterContext context, Func<Task> next)
        {
            var token = (CancellationToken)context.ParameterValue;
            context.ActionContext.CancellationTokens.Add(token);
            return next();
        }
    }
}
