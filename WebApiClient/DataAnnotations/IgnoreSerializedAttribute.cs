using System;
using System.Reflection;

namespace WebApiClient.DataAnnotations
{
    /// <summary>
    /// DataAnnotation
    /// 表示序列时忽略此属性
    /// 默认适用于JsonFormat和KeyValueFormat
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IgnoreSerializedAttribute : DataAnnotationAttribute
    {
        /// <summary>
        /// 执行特性
        /// </summary>
        /// <param name="member">成员</param>
        /// <param name="annotations">注解信息</param>
        public override void Invoke(MemberInfo member, Annotations annotations)
        {
            annotations.IgnoreSerialized = true;
        }
    }
}
