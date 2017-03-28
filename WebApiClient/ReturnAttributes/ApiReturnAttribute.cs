using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    /// <summary>
    /// 表示回复处理抽象类
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method , AllowMultiple = false, Inherited = true)]
    public abstract class ApiReturnAttribute : Attribute
    {
        /// <summary>
        /// 异步获取结果
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract Task<object> GetResultAsync(ApiActionContext context);
    }

}
