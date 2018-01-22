using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient.DataAnnotations
{
    /// <summary>
    /// 表示数据注解特性抽象
    /// </summary>
    public abstract class DataAnnotationAttribute : Attribute
    {
        /// <summary>
        /// 获取或设置适用的序列化范围
        /// </summary>
        public FormatScope Scope { get; set; }

        /// <summary>
        /// 数据注解特性抽象
        /// </summary>
        public DataAnnotationAttribute()
        {
            this.Scope = FormatScope.All;
        }

        /// <summary>
        /// 返回是否声明指定的scope
        /// </summary>
        /// <param name="scope">序列化范围</param>
        /// <returns></returns>
        public bool IsDefinedScope(FormatScope scope)
        {
            return scope == (scope & this.Scope);
        }
    }
}
