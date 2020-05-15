using System;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示请求参数抽象特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public abstract class ApiParameterAttribute : Attribute, IApiParameterAttribute
    {
        /// <summary>
        /// 请求前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="next">下一个执行委托</param>
        /// <returns></returns>
        public async Task BeforeRequestAsync(ApiParameterContext context, Func<Task> next)
        {
            await this.BeforeRequestAsync(context);
            await next();
        }

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param> 
        /// <returns></returns>
        public abstract Task BeforeRequestAsync(ApiParameterContext context);
    }
}
