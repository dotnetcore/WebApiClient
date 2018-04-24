using System;
using System.Linq.Expressions;
using System.Reflection;

namespace WebApiClient
{
    /// <summary>
    /// 表示属性的Getter
    /// </summary>
    class PropertyGetter
    {
        /// <summary>
        /// get方法委托
        /// </summary>
        private readonly Func<object, object> getFunc;

        /// <summary>
        /// 表示属性的Getter
        /// </summary>
        /// <param name="property">属性</param>
        /// <exception cref="ArgumentNullException"></exception>
        public PropertyGetter(PropertyInfo property)
           : this(property?.DeclaringType, property?.Name)
        {
        }

        /// <summary>
        /// 表示类型字段或属性的Getter
        /// </summary>
        /// <param name="declaringType">声名属性的类型</param>
        /// <param name="propertyName">属性的名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        public PropertyGetter(Type declaringType, string propertyName)
        {
            if (declaringType == null)
            {
                throw new ArgumentNullException(nameof(declaringType));
            }

            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }
            this.getFunc = CreateGetterDelegate(declaringType, propertyName);
        }

        /// <summary>
        /// 获取属性的值
        /// </summary>
        /// <param name="instance">实例</param>
        /// <returns></returns>
        public object Invoke(object instance)
        {
            return this.getFunc.Invoke(instance);
        }

        /// <summary>
        /// 创建declaringType类型获取property值的委托
        /// </summary>
        /// <param name="declaringType">实例的类型</param>
        /// <param name="propertyName">属性的名称</param>
        /// <returns></returns>
        private static Func<object, object> CreateGetterDelegate(Type declaringType, string propertyName)
        {
            // (object instance) => (object)((declaringType)instance).propertyName

            var param_instance = Expression.Parameter(typeof(object));
            var body_instance = Expression.Convert(param_instance, declaringType);
            var body_property = Expression.Property(body_instance, propertyName);
            var body_return = Expression.Convert(body_property, typeof(object));

            return Expression.Lambda<Func<object, object>>(body_return, param_instance).Compile();
        }
    }
}
