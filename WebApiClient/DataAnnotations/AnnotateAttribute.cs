using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient.DataAnnotations
{
    /// <summary>
    /// 表示注解特性抽象
    /// </summary>
    public abstract class AnnotateAttribute : Attribute
    {
        /// <summary>
        /// 获取或设置适用范围
        /// </summary>
        public AnnotateScope Scope { get; set; }

        /// <summary>
        /// 返回是否声明指定的scope
        /// </summary>
        /// <param name="scope">适用范围</param>
        /// <returns></returns>
        public bool IsDefinedScope(AnnotateScope scope)
        {
            return scope == (scope & this.Scope);
        }
    }
}
