using System.Reflection;

namespace WebApiClient.DataAnnotations
{
    /// <summary>
    /// 表示注解信息
    /// </summary>
    public class Annotations
    {
        /// <summary>
        /// 获取或设置别名或名称
        /// </summary>
        public string AliasName { get; set; }

        /// <summary>
        /// 获取或设置日期时间格式
        /// </summary>
        public string DateTimeFormat { get; set; }

        /// <summary>
        /// 获取或设置是否忽略序列化
        /// </summary>      
        public bool IgnoreSerialized { get; set; }

        /// <summary>
        /// 获取或设置当值为null时是否忽略序列化
        /// </summary>
        public bool IgnoreWhenNull { get; set; }

        /// <summary>
        /// 返回成员的注解信息
        /// </summary>
        /// <param name="member">成员信息</param>
        /// <param name="scope">序列化范围</param>
        /// <returns></returns>
        public static Annotations GetAnnotations(MemberInfo member, FormatScope scope)
        {
            var annotations = new Annotations();
            var attributes = member.GetCustomAttributes<DataAnnotationAttribute>(true);
            foreach (var item in attributes)
            {
                if (item.IsDefinedScope(scope) == true)
                {
                    item.Invoke(member, annotations);
                }
            }
            return annotations;
        }
    }
}
