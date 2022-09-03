using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示仅用于给Action或者Filter传递参数，不用于接口传递。
    /// </summary>
    public class IgnoreAttribute : ApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override Task OnRequestAsync(ApiParameterContext context)
        {
            return Task.CompletedTask;
        }
    }
}
