using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WebApiClient
{
    /// <summary>
    /// 表示属性
    /// </summary>
    class KeyValueProperty
    {
        /// <summary>
        /// 获取器
        /// </summary>
        private readonly Method geter;

        /// <summary>
        /// 获取属性别名或名称
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// 获取属性信息
        /// </summary>
        public PropertyInfo Info { get; private set; }

        /// <summary>
        /// 获取是否声明KeyValueIgnoreAttribute
        /// </summary>
        public bool IsKeyValueIgnore { get; private set; }

        /// <summary>
        /// 属性
        /// </summary>
        /// <param name="property">属性信息</param>
        public KeyValueProperty(PropertyInfo property)
        {
            this.Info = property;
            this.IsKeyValueIgnore = property.IsDefined(typeof(KeyValueIgnoreAttribute));

            var keyAlias = property.GetAttribute<KeyAliasAttribute>(true);
            this.Name = keyAlias == null ? property.Name : keyAlias.Alias;

            var getMethod = property.GetGetMethod();
            if (getMethod != null)
            {
                this.geter = new Method(getMethod);
            }
        }

        /// <summary>
        /// 获取属性的值
        /// </summary>
        /// <param name="instance">实例</param>
        /// <returns></returns>
        public object GetValue(object instance)
        {
            return this.geter.Invoke(instance, null);
        }

        /// <summary>
        /// 类型属性的Setter缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type, KeyValueProperty[]> cached = new ConcurrentDictionary<Type, KeyValueProperty[]>();

        /// <summary>
        /// 从类型的属性获取属性
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static KeyValueProperty[] GetProperties(Type type)
        {
            return cached.GetOrAdd(type, t =>
              t.GetProperties().Select(p => new KeyValueProperty(p)).ToArray()
           );
        }

        /// <summary>
        /// 表示方法
        /// </summary>
        private class Method
        {
            /// <summary>
            /// 方法执行委托
            /// </summary>
            private readonly Func<object, object[], object> invoker;

            /// <summary>
            /// 获取方法名
            /// </summary>
            public string Name { get; protected set; }

            /// <summary>
            /// 获取方法信息
            /// </summary>
            public MethodInfo Info { get; private set; }

            /// <summary>
            /// 方法
            /// </summary>
            /// <param name="method">方法信息</param>
            public Method(MethodInfo method)
            {
                this.Name = method.Name;
                this.Info = method;
                this.invoker = Method.CreateInvoker(method);
            }

            /// <summary>
            /// 执行方法
            /// </summary>
            /// <param name="instance">实例</param>
            /// <param name="parameters">参数</param>
            /// <returns></returns>
            public object Invoke(object instance, params object[] parameters)
            {
                return this.invoker.Invoke(instance, parameters);
            }

            /// <summary>
            /// 生成方法的调用委托
            /// </summary>
            /// <param name="method">方法成员信息</param>
            /// <exception cref="ArgumentException"></exception>
            /// <returns></returns>
            private static Func<object, object[], object> CreateInvoker(MethodInfo method)
            {
                var instance = Expression.Parameter(typeof(object), "instance");
                var parameters = Expression.Parameter(typeof(object[]), "parameters");

                var instanceCast = method.IsStatic ? null : Expression.Convert(instance, method.ReflectedType);
                var parametersCast = method.GetParameters().Select((p, i) =>
                {
                    var parameter = Expression.ArrayIndex(parameters, Expression.Constant(i));
                    return Expression.Convert(parameter, p.ParameterType);
                });

                var body = Expression.Call(instanceCast, method, parametersCast);

                if (method.ReturnType == typeof(void))
                {
                    var action = Expression.Lambda<Action<object, object[]>>(body, instance, parameters).Compile();
                    return (_instance, _parameters) =>
                    {
                        action.Invoke(_instance, _parameters);
                        return null;
                    };
                }
                else
                {
                    var bodyCast = Expression.Convert(body, typeof(object));
                    return Expression.Lambda<Func<object, object[], object>>(bodyCast, instance, parameters).Compile();
                }
            }
        }
    }
}