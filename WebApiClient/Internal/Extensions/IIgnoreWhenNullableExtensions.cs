using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// IIgnoreWhenNullable接口扩展
    /// </summary>
    static class IIgnoreWhenNullableExtensions
    {
        /// <summary>
        /// 返回相对parameter的value值，是否应该忽略
        /// </summary>
        /// <param name="able"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static bool IsIgnoreWith(this IIgnoreWhenNullable able, ApiParameterDescriptor parameter)
        {
            return able.IsIgnoreWith(parameter.Value);
        }

        /// <summary>
        /// 返回相对value值，是否应该忽略
        /// </summary>
        /// <param name="able"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsIgnoreWith(this IIgnoreWhenNullable able, object value)
        {
            return able.IgnoreWhenNull == true && value == null;
        }
    }
}
