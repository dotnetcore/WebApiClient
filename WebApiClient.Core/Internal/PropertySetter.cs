using System;
using System.Linq.Expressions;
using System.Reflection;

namespace WebApiClient
{
    /// <summary>
    /// 表示属性的设置器
    /// </summary>
    class PropertySetter
    {
        /// <summary>
        /// set方法委托
        /// </summary>
        private readonly Action<object, object> setFunc;

        /// <summary>
        /// 表示属性的Getter
        /// </summary>
        /// <param name="property">属性</param>
        /// <exception cref="ArgumentNullException"></exception>
        public PropertySetter(PropertyInfo property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }
            this.setFunc = CreateSetterDelegate(property);
        }

        /// <summary>
        /// 设置属性的值
        /// </summary>
        /// <param name="instance">实例</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public void Invoke(object instance, object value)
        {
            this.setFunc.Invoke(instance, value);
        }

        /// <summary>
        /// 创建属性的Set委托
        /// </summary>
        /// <param name="property">属性</param>
        /// <returns></returns>
        private static Action<object, object> CreateSetterDelegate(PropertyInfo property)
        {
            // (object instance, object value) => 
            //     ((instanceType)instance).Set_XXX((propertyType)value)

            var param_instance = Expression.Parameter(typeof(object));
            var param_value = Expression.Parameter(typeof(object));

            var body_instance = Expression.Convert(param_instance, property.DeclaringType);
            var body_value = Expression.Convert(param_value, property.PropertyType);
            var body_call = Expression.Call(body_instance, property.GetSetMethod(), body_value);

            return Expression.Lambda<Action<object, object>>(body_call, param_instance, param_value).Compile();
        }
    }
}
