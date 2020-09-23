using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示参数类型为CancellationToken处理特性
    /// </summary>
    class CancellationTokenTypeAttribute : ApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override Task OnRequestAsync(ApiParameterContext context)
        {
            if (context.ParameterValue is CancellationToken token)
            {
                context.HttpContext.CancellationTokens.Add(token);
            }
            else if (context.ParameterValue is IEnumerable<CancellationToken> tokens)
            {
                foreach (var item in tokens)
                {
                    context.HttpContext.CancellationTokens.Add(item);
                }
            }
            return Task.CompletedTask;
        }
    }
}
