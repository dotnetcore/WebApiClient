using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示与Api方法关联的特性
    /// </summary>
    public abstract class ApiActionAttribute : Attribute
    {
        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        public abstract void BeforeRequest(ApiActionContext context);
    }
}
