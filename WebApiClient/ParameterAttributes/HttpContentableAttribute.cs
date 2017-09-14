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
    /// 表示参数内容为IHttpContentable对象
    /// 不可继承
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class HttpContentableAttribute : ApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        public override async Task BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var method = context.RequestMessage.Method;
            if (method == HttpMethod.Get || method == HttpMethod.Head)
            {
                return;
            }

            var contentables = this.GetHttpContentables(parameter);
            foreach (var item in contentables)
            {
                item.SetRquestHttpContent(context, parameter);
            }
            await TaskExtend.CompletedTask;
        }

        /// <summary>
        /// 从参数值获取IHttpContentable对象
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private IEnumerable<IHttpContentable> GetHttpContentables(ApiParameterDescriptor parameter)
        {
            if (parameter.Value == null)
            {
                yield break;
            }

            var contenable = parameter.Value as IHttpContentable;
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
                var able = item as IHttpContentable;
                if (able != null)
                {
                    yield return able;
                }
            }
        }

    }
}
