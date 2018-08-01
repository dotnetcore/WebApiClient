using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 提供属性值输入合法性验证
    /// </summary>
    static class PropertyValidator
    {
        /// <summary>
        /// 类型的属性否需要验证缓存
        /// </summary>
        private static readonly ConcurrentCache<Type, bool> cache = new ConcurrentCache<Type, bool>();

        /// <summary>
        /// 验证参数值的属性输入合法性
        /// </summary>
        /// <param name="parameter">参数描述</param>
        /// <exception cref="ValidationException"></exception>
        /// <returns>返回是否已验证</returns>
        public static bool Validate(ApiParameterDescriptor parameter)
        {
            var instance = parameter.Value;
            if (instance == null)
            {
                return false;
            }

            var type = instance.GetType();
            if (type == typeof(string) || type.GetTypeInfo().IsValueType == true)
            {
                return false;
            }

            var needValidate = cache.GetOrAdd(type, t => t.GetProperties().Any(p => p.CanRead && p.IsDefined(typeof(ValidationAttribute), true)));
            if (needValidate == false)
            {
                return false;
            }

            var ctx = new ValidationContext(instance)
            {
                MemberName = parameter.Name
            };
            Validator.ValidateObject(parameter.Value, ctx, true);

            return true;
        }
    }
}
