using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示参数内容为CancellationToken处理特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    class CancellationTokenAttribute : Attribute, IApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public Task BeforeRequestAsync(ApiParameterContext context)
        {
            var token = (CancellationToken)context.ParameterValue;
            context.ActionContext.CancellationTokens.Add(token);
            return Task.CompletedTask;
        }
    }
}
