using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient.DataAnnotations
{
    /// <summary>
    /// DataAnnotation
    /// 表示序列时属性值为null则忽略
    /// 默认适用于KeyValueFormat
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class IgnoreWhenNullAttribute : DataAnnotationAttribute
    {
        /// <summary>
        /// 值为null此属性将忽略
        /// 默认适用于KeyValueFormat
        /// </summary>
        public IgnoreWhenNullAttribute()
        {
            this.Scope = FormatScope.KeyValueFormat;
        }
    }
}
