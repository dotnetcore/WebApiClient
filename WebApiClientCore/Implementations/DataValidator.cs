using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 数据验证器
    /// 提供返回值的属性验证、参数值和参数的属性值验证
    /// </summary>
    static class DataValidator
    {
        /// <summary>
        /// 类型的属性否需要验证缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type, bool> cache = new();

        /// <summary>
        /// 验证参数值输入合法性
        /// 验证参数的属性值输入合法性
        /// </summary>
        /// <param name="parameter">参数描述</param>
        /// <param name="parameterValue">参数值</param>
        /// <param name="validateProperty">是否验证属性值</param>
        /// <exception cref="ValidationException"></exception>
        [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026", Justification = "允许 parameterValue 属性被裁剪")]
        public static void ValidateParameter(ApiParameterDescriptor parameter, object? parameterValue, bool validateProperty)
        {
            var name = parameter.Name;
            foreach (var validation in parameter.ValidationAttributes)
            {
                validation.Validate(parameterValue, name);
            }

            if (validateProperty && parameterValue != null && IsNeedValidateProperty(parameterValue) == true)
            {
                var ctx = new ValidationContext(parameterValue) { MemberName = name };
                Validator.ValidateObject(parameterValue, ctx, true);
            }
        }

        /// <summary>
        /// 验证参返回的结果
        /// </summary>
        /// <param name="value">结果值</param> 
        /// <exception cref="ValidationException"></exception>
        [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026", Justification = "允许 value 属性被裁剪")]
        public static void ValidateReturnValue(object? value)
        {
            if (value != null && IsNeedValidateProperty(value) == true)
            {
                var ctx = new ValidationContext(value);
                Validator.ValidateObject(value, ctx, true);
            }
        }

        /// <summary>
        /// 返回是否需要进行属性验证
        /// </summary>
        /// <param name="instance">实例</param>
        /// <returns></returns>
        [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2070", Justification = "允许 instance 属性被裁剪")]
        private static bool IsNeedValidateProperty(object instance)
        {
            var type = instance.GetType();
            if (type == typeof(string) || type.IsValueType == true)
            {
                return false;
            }

            return cache.GetOrAdd(type, t => t.GetProperties().Any(p => p.CanRead && p.IsDefined(typeof(ValidationAttribute), true)));
        }
    }
}
