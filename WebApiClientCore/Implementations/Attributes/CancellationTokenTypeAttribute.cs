using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClientCore.Implementations.Attributes
{
    /// <summary>
    /// 表示参数类型为CancellationToken处理特性
    /// </summary>
    sealed class CancellationTokenTypeAttribute : IApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public Task OnRequestAsync(ApiParameterContext context)
        {
            if (context.ParameterValue is CancellationToken token)
            {
                if (token.Equals(CancellationToken.None) == false)
                {
                    context.HttpContext.CancellationTokens.Add(token);
                }
            }
            else if (context.ParameterValue is IEnumerable<CancellationToken> tokens)
            {
                foreach (var item in tokens)
                {
                    if (token.Equals(CancellationToken.None) == false)
                    {
                        context.HttpContext.CancellationTokens.Add(item);
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
