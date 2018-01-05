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
    /// 表示方法
    /// </summary>
    class Method
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
