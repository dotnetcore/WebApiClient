using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace WebApiClient.Contexts
{
    /// <summary>
    /// 表示请求Api描述
    /// </summary>
    [DebuggerDisplay("Name = {Name}")]
    public class ApiActionDescriptor 
    {
        /// <summary>
        /// 获取Api名称
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// 获取关联的方法信息
        /// </summary>
        public MethodInfo Member { get; internal set; }

        /// <summary>
        /// 获取Api关联的特性
        /// </summary>
        public IApiActionAttribute[] Attributes { get; internal set; }

        /// <summary>
        /// 获取Api关联的过滤器特性
        /// </summary>
        public IApiActionFilterAttribute[] Filters { get; internal set; }

        /// <summary>
        /// 获取Api的参数描述
        /// </summary>
        public ApiParameterDescriptor[] Parameters { get; internal set; }

        /// <summary>
        /// 获取Api的返回描述
        /// </summary>
        public ApiReturnDescriptor Return { get; internal set; }      

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public ApiActionDescriptor Clone()
        {
            return new ApiActionDescriptor
            {
                Name = this.Name,
                Member = this.Member,
                Return = this.Return,
                Filters = this.Filters,
                Attributes = this.Attributes,
                Parameters = this.Parameters.Select(item => item.Clone()).ToArray()
            };
        }
    }
}
