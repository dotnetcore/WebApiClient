using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Attributes;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示参数内容为IApiParameterable对象或其数组
    /// 不可继承
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class ApiParameterableAttribute : ApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        public override async Task BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var ables = this.GetApiParameterables(parameter);
            foreach (var item in ables)
            {
                await item.BeforeRequestAsync(context, parameter);
            }
        }

        /// <summary>
        /// 从参数值获取IApiParameterable对象
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private IEnumerable<IApiParameterable> GetApiParameterables(ApiParameterDescriptor parameter)
        {
            if (parameter.Value == null)
            {
                yield break;
            }

            var contenable = parameter.Value as IApiParameterable;
            if (contenable != null)
            {
                yield return contenable;
                yield break;
            }

            var enumerable = parameter.Value as IEnumerable;
            if (enumerable == null)
            {
                yield break;
            }

            foreach (var item in enumerable)
            {
                var able = item as IApiParameterable;
                if (able != null)
                {
                    yield return able;
                }
            }
        }

    }
}
