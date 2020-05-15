using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示参数内容为IApiParameterable对象或其数组
    /// </summary>
    class ParameterableAttribute : Attribute, IApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task BeforeRequestAsync(ApiParameterContext context, Func<Task> next)
        {
            if (context.ParameterValue is IApiParameterable able)
            {
                await able.BeforeRequestAsync(context).ConfigureAwait(false);
            }
            else if (context.ParameterValue is IEnumerable<IApiParameterable> array)
            {
                foreach (var item in array)
                {
                    await item.BeforeRequestAsync(context).ConfigureAwait(false);
                }
            }
            await next();
        }
    }
}
