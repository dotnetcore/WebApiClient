using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WebApiClientCore
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

            var paramInstance = Expression.Parameter(typeof(TDeclaring));
            var paramValue = Expression.Parameter(typeof(TProperty));

            var bodyInstance = (Expression)paramInstance;
            var bodyValue = (Expression)paramValue;

            if (typeof(TDeclaring) != property.DeclaringType)
            {
                bodyInstance = Expression.Convert(bodyInstance, property.DeclaringType);
            }

            if (typeof(TProperty) != property.PropertyType)
            {
                bodyValue = Expression.Convert(bodyValue, property.PropertyType);
            }

            var bodyCall = Expression.Call(bodyInstance, property.GetSetMethod(), bodyValue);
            return Expression.Lambda<Action<TDeclaring, TProperty>>(bodyCall, paramInstance, paramValue).Compile();
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

            if (property.DeclaringType == null)
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
        public static Func<TDeclaring, TProperty> CreateGetFunc<TDeclaring, TProperty>(Type declaringType, string propertyName, Type? propertyType = null)
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

            var paramInstance = Expression.Parameter(typeof(TDeclaring));

            var bodyInstance = (Expression)paramInstance;
            if (typeof(TDeclaring) != declaringType)
            {
                bodyInstance = Expression.Convert(bodyInstance, declaringType);
            }

            var bodyProperty = (Expression)Expression.Property(bodyInstance, propertyName);
            if (typeof(TProperty) != propertyType)
            {
                bodyProperty = Expression.Convert(bodyProperty, typeof(TProperty));
            }

            return Expression.Lambda<Func<TDeclaring, TProperty>>(bodyProperty, paramInstance).Compile();
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

            var paramInstance = Expression.Parameter(typeof(TDeclaring));

            var bodyInstance = (Expression)paramInstance;
            if (typeof(TDeclaring) != field.DeclaringType)
            {
                bodyInstance = Expression.Convert(bodyInstance, field.DeclaringType);
            }

            var bodyField = (Expression)Expression.Field(bodyInstance, field);
            if (typeof(TField) != field.FieldType)
            {
                bodyField = Expression.Convert(bodyField, typeof(TField));
            }

            return Expression.Lambda<Func<TDeclaring, TField>>(bodyField, paramInstance).Compile();
        }


        /// <summary>
        /// 创建类型的构造器调用委托
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static Func<TType> CreateCtorFunc<TType>(Type type)
        {
            return CreateCtorFunc<Func<TType>>(type, Array.Empty<Type>());
        }

        /// <summary>
        /// 创建类型的构造器调用委托
        /// </summary>
        /// <typeparam name="TArg1">第一个参数类型</typeparam>
        /// <typeparam name="TType">类型</typeparam>
        /// <param name="type">类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static Func<TArg1, TType> CreateCtorFunc<TArg1, TType>(Type type)
        {
            var args = new Type[] { typeof(TArg1) };
            return CreateCtorFunc<Func<TArg1, TType>>(type, args);
        }

        /// <summary>
        /// 创建类型的构造器调用委托
        /// </summary>
        /// <typeparam name="TArg1">第一个参数类型</typeparam>
        /// <typeparam name="TArg2">第二个参数类型</typeparam>
        /// <typeparam name="TType">类型</typeparam>
        /// <param name="type">类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static Func<TArg1, TArg2, TType> CreateCtorFunc<TArg1, TArg2, TType>(Type type)
        {
            var args = new Type[] { typeof(TArg1), typeof(TArg2) };
            return CreateCtorFunc<Func<TArg1, TArg2, TType>>(type, args);
        }

        /// <summary>
        /// 创建类型的构造器调用委托
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TType"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Func<TArg1, TArg2, TArg3, TType> CreateCtorFunc<TArg1, TArg2, TArg3, TType>(Type type)
        {
            var args = new Type[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) };
            return CreateCtorFunc<Func<TArg1, TArg2, TArg3, TType>>(type, args);
        }

        /// <summary>
        /// 创建类型的构造器调用委托
        /// </summary>
        /// <typeparam name="TFunc">构造器调用委托</typeparam>
        /// <param name="type">类型</param>
        /// <param name="args">参数类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        private static TFunc CreateCtorFunc<TFunc>(Type type, Type[] args)
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
            if (ctor == null)
            {
                var argTypeNames = string.Join(", ", args.Select(a => a.Name));
                throw new ArgumentException(Resx.missing_Ctor.Format(type, argTypeNames));
            }

            var parameters = args.Select(t => Expression.Parameter(t)).ToArray();
            var body = Expression.New(ctor, parameters);
            return Expression.Lambda<TFunc>(body, parameters).Compile();
        }
    }
}