using Mono.Cecil;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebApiClient.AOT.Task
{
    /// <summary>
    /// 表示接口
    /// </summary>
    class Interface
    {
        /// <summary>
        /// 获取接口类型
        /// </summary>
        public TypeDefinition Type { get; private set; }

        /// <summary>
        /// 表示接口
        /// </summary>
        /// <param name="interface">接口类型</param>
        public Interface(TypeDefinition @interface)
        {
            this.Type = @interface;
        }

        /// <summary>
        /// 返回TypeReference
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public TypeReference GetTypeReference(Type type)
        {
            return this.Type.Module.ImportReference(type);
        }

        /// <summary>
        /// 返回MethodReference
        /// </summary>
        /// <param name="method">方法</param>
        /// <returns></returns>
        public MethodReference GetMethodReference(MethodBase method)
        {
            return this.Type.Module.ImportReference(method);
        }

        /// <summary>
        /// 获取接口类型及其继承的接口的所有方法
        /// 忽略IHttpApi接口的方法
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public MethodDefinition[] GetAllApis()
        {
            var excepts = this.GetTypeReference(typeof(HttpApiClient))
                .Resolve()
                .Interfaces
                .Select(item => item.InterfaceType.Resolve());

            var interfaces = new[] { this.Type }.Concat(this.Type.Interfaces.Select(i => i.InterfaceType.Resolve()))
                .Except(excepts, TypeDefinitionComparer.Instance)
                .ToArray();

            var apiMethods = interfaces.SelectMany(item => item.Methods).ToArray();
            foreach (var method in apiMethods)
            {
                this.EnsureApiMethod(method);
            }
            return apiMethods;
        }

        /// <summary>
        /// 确保方法是支持的Api接口
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        private void EnsureApiMethod(MethodDefinition method)
        {
            if (method.IsGenericInstance == true)
            {
                throw new NotSupportedException("不支持泛型方法：" + method);
            }

            if (method.IsSpecialName == true)
            {
                throw new NotSupportedException("不支持属性访问器：" + method);
            }

            var genericType = method.ReturnType;
            if (genericType.IsGenericInstance == true)
            {
                genericType = genericType.GetElementType();
            }

            var isTaskType = this.TypeReferenceEquals(genericType, typeof(Task<>)) || this.TypeReferenceEquals(genericType, typeof(ITask<>));
            if (isTaskType == false)
            {
                var message = $"返回类型必须为Task<>或ITask<>：{method}";
                throw new NotSupportedException(message);
            }

            foreach (var parameter in method.Parameters)
            {
                if (parameter.ParameterType.IsByReference == true)
                {
                    var message = $"接口参数不支持ref/out修饰：{parameter}";
                    throw new NotSupportedException(message);
                }
            }
        }

        /// <summary>
        /// 比较类型是否相等
        /// </summary>
        /// <param name="typeReference"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool TypeReferenceEquals(TypeReference typeReference, Type type)
        {
            return typeReference.FullName == this.GetTypeReference(type).FullName;
        }
    }
}
