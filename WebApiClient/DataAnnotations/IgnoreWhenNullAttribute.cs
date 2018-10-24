using System;
using System.Reflection;

namespace WebApiClient.DataAnnotations
{
    /// <summary>
    /// DataAnnotation
    /// 表示序列时属性值为null则忽略
    /// 默认适用于KeyValueFormat
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IgnoreWhenNullAttribute : DataAnnotationAttribute
    {
        /// <summary>
        /// 值为null此属性将忽略
        /// 默认适用于KeyValueFormat
        /// </summary>
        public IgnoreWhenNullAttribute()
        {
            this.Scope = FormatScope.KeyValueFormat;
        }

        /// <summary>
        /// 执行特性
        /// </summary>
        /// <param name="member">成员</param>
        /// <param name="annotations">注解信息</param>
        public override void Invoke(MemberInfo member, Annotations annotations)
        {
            annotations.IgnoreWhenNull = true;
        }
    }
}
