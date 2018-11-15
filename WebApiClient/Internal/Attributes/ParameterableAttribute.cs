using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 表示参数内容为IApiParameterable对象或其数组
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    class ParameterableAttribute : Attribute, IApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        public async Task BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            if (parameter.Value is IApiParameterable able)
            {
                await able.BeforeRequestAsync(context, parameter).ConfigureAwait(false);
            }
            else if (parameter.Value is IEnumerable<IApiParameterable> array)
            {
                foreach (var item in array)
                {
                    await item.BeforeRequestAsync(context, parameter).ConfigureAwait(false);
                }
            }
        }
    }
}
