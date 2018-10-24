using System;
using System.Reflection;

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
            if (this.Scope == FormatScope.All)
            {
                return true;
            }
            return this.Scope.HasFlag(scope);
        }

        /// <summary>
        /// 执行特性
        /// </summary>
        /// <param name="member">成员</param>
        /// <param name="annotations">注解信息</param>
        public abstract void Invoke(MemberInfo member, Annotations annotations);
    }
}
