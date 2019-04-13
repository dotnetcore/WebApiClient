using System;
using System.Reflection;

namespace WebApiClient.DataAnnotations
{
    /// <summary>
    /// DataAnnotation
    /// 表示将参数别名或序列时此属性的别名
    /// 当修饰属性时默认适用于JsonFormat和KeyValueFormat
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AliasAsAttribute : DataAnnotationAttribute
    {
        /// <summary>
        /// 获取别名
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 指定参数或属性的别名
        /// </summary>
        /// <param name="name">参数或属性的别名</param>
        /// <exception cref="ArgumentNullException"></exception>
        public AliasAsAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name) == true)
            {
                throw new ArgumentNullException(nameof(name));
            }
            this.Name = name;
        }

        /// <summary>
        /// 执行特性
        /// </summary>
        /// <param name="member">成员</param>
        /// <param name="annotations">注解信息</param>
        public override void Invoke(MemberInfo member, Annotations annotations)
        {
            annotations.AliasName = this.Name;
        }
    }
}
