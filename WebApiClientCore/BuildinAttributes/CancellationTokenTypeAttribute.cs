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
            var token = (CancellationToken)context.ParameterValue;
            context.CancellationTokens.Add(token);
            return Task.CompletedTask;
        }
    }
}
