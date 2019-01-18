using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// Api验证器，提供返回值的属性验证、参数值和参数的属性值验证
    /// </summary>
    static class ApiValidator
    {
        /// <summary>
        /// 类型的属性否需要验证缓存
        /// </summary>
        private static readonly ConcurrentCache<Type, bool> cache = new ConcurrentCache<Type, bool>();

        /// <summary>
        /// 返回是否需要进行属性验证
        /// </summary>
        /// <param name="instance">实例</param>
        /// <returns></returns>
        private static bool IsNeedValidateProperty(object instance)
        {
            if (instance == null)
            {
                return false;
            }

            var type = instance.GetType();
            if (type == typeof(string) || type.GetTypeInfo().IsValueType == true)
            {
                return false;
            }

            return cache.GetOrAdd(type, t => t.GetProperties().Any(p => p.CanRead && p.IsDefined(typeof(ValidationAttribute), true)));
        }

        /// <summary>
        /// 验证参数值输入合法性
        /// 验证参数的属性值输入合法性
        /// </summary>
        /// <param name="parameter">参数描述</param>
        /// <param name="validateProperty">是否验证属性值</param>
        /// <exception cref="ValidationException"></exception>
        public static void ValidateParameter(ApiParameterDescriptor parameter, bool validateProperty)
        {
            var name = parameter.Name;
            var instance = parameter.Value;

            foreach (var validation in parameter.ValidationAttributes)
            {
                validation.Validate(instance, name);
            }

            if (validateProperty == true && IsNeedValidateProperty(instance) == true)
            {
                var ctx = new ValidationContext(instance) { MemberName = name };
                Validator.ValidateObject(instance, ctx, true);
            }
        }

        /// <summary>
        /// 验证参返回的结果
        /// </summary>
        /// <param name="value">结果值</param>
        /// <param name="validateProperty">是否验证属性值</param>
        /// <exception cref="ValidationException"></exception>
        public static void ValidateReturnValue(object value, bool validateProperty)
        {
            if (validateProperty == true && IsNeedValidateProperty(value) == true)
            {
                var ctx = new ValidationContext(value);
                Validator.ValidateObject(value, ctx, true);
            }
        }
    }
}
