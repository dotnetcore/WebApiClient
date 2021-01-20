using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApiClientCore.Internals.Attributes
{
    /// <summary>
    /// 表示参数类型为IApiParameter的处理特性
    /// </summary>
    sealed class ApiParameterTypeAttribute : IApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param> 
        /// <returns></returns>
        public async Task OnRequestAsync(ApiParameterContext context)
        {
            if (context.ParameterValue is IApiParameter parameter)
            {
                await parameter.OnRequestAsync(context).ConfigureAwait(false);
            }
            else if (context.ParameterValue is IEnumerable<IApiParameter> parameters)
            {
                foreach (var item in parameters)
                {
                    await item.OnRequestAsync(context).ConfigureAwait(false);
                }
            }
        }
    }
}
