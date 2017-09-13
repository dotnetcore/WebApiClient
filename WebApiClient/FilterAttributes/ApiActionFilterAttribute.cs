using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示请求Api过滤器特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class ApiActionFilterAttribute : Attribute
    {
        /// <summary>
        /// 准备请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public virtual Task OnBeginRequestAsync(ApiActionContext context)
        {
            return TaskExtend.CompletedTask;
        }

        /// <summary>
        /// 请求完成之后
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public virtual Task OnEndRequestAsync(ApiActionContext context)
        {
            return TaskExtend.CompletedTask;
        }
    }
}
