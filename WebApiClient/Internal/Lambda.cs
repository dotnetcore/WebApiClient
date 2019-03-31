using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WebApiClient
{
    /// <summary>
    /// 表示式委托
    /// </summary>
    static class Lambda
    {
        /// <summary>
        /// 创建属性的设置委托
        /// </summary>
        /// <typeparam name="TDeclaring"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property">属性</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Action<TDeclaring, TProperty> CreateSetAction<TDeclaring, TProperty>(PropertyInfo property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            // (TDeclaring instance, TProperty value) => 
            //    ((declaringType)instance).Set_XXX( (propertyType)value )

            var param_instance = Expression.Parameter(typeof(TDeclaring));
            var param_value = Expression.Parameter(typeof(TProperty));

            var bodyInstance = (Expression)param_instance;
            var body_value = (Expression)param_value;

            if (typeof(TDeclaring) != property.DeclaringType)
            {
                bodyInstance = Expression.Convert(bodyInstance, property.DeclaringType);
            }

            if (typeof(TProperty) != property.PropertyType)
            {
                body_value = Expression.Convert(body_value, property.PropertyType);
            }

            var body_call = Expression.Call(bodyInstance, property.GetSetMethod(), body_value);
            return Expression.Lambda<Action<TDeclaring, TProperty>>(body_call, param_instance, param_value).Compile();
        }


        /// <summary>
        /// 创建属性的获取委托
        /// </summary>
        /// <typeparam name="TDeclaring"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property">属性</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Func<TDeclaring, TProperty> CreateGetFunc<TDeclaring, TProperty>(PropertyInfo property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            return CreateGetFunc<TDeclaring, TProperty>(property.DeclaringType, property.Name, property.PropertyType);
        }

        /// <summary>
        /// 创建属性的获取委托
        /// </summary>
        /// <typeparam name="TDeclaring"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="declaringType">实例的类型</param>
        /// <param name="propertyName">属性的名称</param>
        /// <param name="propertyType">属性的类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Func<TDeclaring, TProperty> CreateGetFunc<TDeclaring, TProperty>(Type declaringType, string propertyName, Type propertyType = null)
        {
            if (declaringType == null)
            {
                throw new ArgumentNullException(nameof(declaringType));
            }

            if (string.IsNullOrEmpty(propertyName) == true)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            // (TDeclaring instance) => (propertyType)((declaringType)instance).propertyName

            var param_instance = Expression.Parameter(typeof(TDeclaring));

            var body_instance = (Expression)param_instance;
            if (typeof(TDeclaring) != declaringType)
            {
                body_instance = Expression.Convert(body_instance, declaringType);
            }

            var body_property = (Expression)Expression.Property(body_instance, propertyName);
            if (typeof(TProperty) != propertyType)
            {
                body_property = Expression.Convert(body_property, typeof(TProperty));
            }

            return Expression.Lambda<Func<TDeclaring, TProperty>>(body_property, param_instance).Compile();
        }


        /// <summary>
        /// 创建字段的获取委托
        /// </summary>
        /// <typeparam name="TDeclaring"></typeparam>
        /// <typeparam name="TField"></typeparam>
        /// <param name="field">字段</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Func<TDeclaring, TField> CreateGetFunc<TDeclaring, TField>(FieldInfo field)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            // (TDeclaring instance) => (fieldType)((declaringType)instance).fieldName

            var param_instance = Expression.Parameter(typeof(TDeclaring));

            var body_instance = (Expression)param_instance;
            if (typeof(TDeclaring) != field.DeclaringType)
            {
                body_instance = Expression.Convert(body_instance, field.DeclaringType);
            }

            var body_field = (Expression)Expression.Field(body_instance, field);
            if (typeof(TField) != field.FieldType)
            {
                body_field = Expression.Convert(body_field, typeof(TField));
            }

            return Expression.Lambda<Func<TDeclaring, TField>>(body_field, param_instance).Compile();
        }


        /// <summary>
        /// 创建类型的创建工厂
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static Func<TType> CreateNewFunc<TType>(Type type)
        {
            var args = TypeExtensions.EmptyTypes;
            return CreateNewFactory<Func<TType>>(type, args);
        }

        /// <summary>
        /// 创建类型的创建工厂
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <typeparam name="T1">第一个参数类型</typeparam>
        /// <param name="type">类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static Func<T1, TType> CreateNewFunc<TType, T1>(Type type)
        {
            var args = new Type[] { typeof(T1) };
            return CreateNewFactory<Func<T1, TType>>(type, args);
        }

        /// <summary>
        /// 创建类型的创建工厂
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <typeparam name="T1">第一个参数类型</typeparam>
        /// <typeparam name="T2">第二个参数类型</typeparam>
        /// <param name="type">类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static Func<T1, T2, TType> CreateNewFunc<TType, T1, T2>(Type type)
        {
            var args = new Type[] { typeof(T1), typeof(T2) };
            return CreateNewFactory<Func<T1, T2, TType>>(type, args);
        }

        /// <summary>
        /// 创建类型的创建工厂
        /// </summary>
        /// <typeparam name="TFunc"></typeparam>
        /// <param name="type">类型</param>
        /// <param name="args">参数类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        private static TFunc CreateNewFactory<TFunc>(Type type, Type[] args)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var ctor = type.GetConstructor(args);
            var parameters = args.Select(t => Expression.Parameter(t)).ToArray();
            var body = Expression.New(ctor, parameters);
            return Expression.Lambda<TFunc>(body, parameters).Compile();
        }
    }
}