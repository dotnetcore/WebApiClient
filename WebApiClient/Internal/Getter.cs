using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示属性的Getter
    /// </summary>
    class Getter
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
        public Getter(PropertyInfo property)
           : this(property.DeclaringType, property.Name)
        {
        } 

        /// <summary>
        /// 表示类型字段或属性的Getter
        /// </summary>
        /// <param name="declaringType">声名属性的类型</param>
        /// <param name="propertyName">属性的名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Getter(Type declaringType, string propertyName)
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
        /// 创建objectType类型获取propertyName的委托
        /// </summary>
        /// <param name="instanceType">实例的类型</param>
        /// <param name="propertyName">属性的名称</param>
        /// <returns></returns>
        private static Func<object, object> CreateGetterDelegate(Type instanceType, string propertyName)
        {
            // (object arg) => (object)((instanceType)arg).propertyName

            var arg = Expression.Parameter(typeof(object));

            var castArg = Expression.Convert(arg, instanceType);
            var propertyAccess = Expression.Property(castArg, propertyName);
            var objectResult = Expression.Convert(propertyAccess, typeof(object));

            return Expression.Lambda<Func<object, object>>(objectResult, arg).Compile();
        }
    }
}
