using System.Threading;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示参数内容为CancellationToken处理特性
    /// </summary>
    class CancellationTokenAttribute : ApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override Task OnRequestAsync(ApiParameterContext context)
        {
            var token = (CancellationToken)context.ParameterValue;
            context.RequestContext.CancellationTokens.Add(token);
            return Task.CompletedTask;
        }
    }
}
