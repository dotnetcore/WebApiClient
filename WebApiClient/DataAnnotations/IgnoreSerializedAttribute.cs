using System;

namespace WebApiClient.DataAnnotations
{
    /// <summary>
    /// DataAnnotation
    /// 表示序列时忽略此属性
    /// 默认适用于JsonFormat和KeyValueFormat
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class IgnoreSerializedAttribute : DataAnnotationAttribute
    {
        /// <summary>
        /// 当序列化对象时忽略此属性
        /// </summary>
        public IgnoreSerializedAttribute()
        {
            this.Scope = AnnotateScope.JsonFormat | AnnotateScope.KeyValueFormat;
        }
    }
}
