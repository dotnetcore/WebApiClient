using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// ApiAction修饰特性抽象
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class ApiActionAttribute : Attribute, IApiActionAttribute
    {
        /// <summary>
        /// 获取本类型是否允许在接口与方法上重复
        /// </summary>
        public virtual bool AllowMultiple { get; private set; }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract Task BeforeRequestAsync(ApiActionContext context);
    }
}
