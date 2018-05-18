using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mono.Cecil.Rocks;

namespace WebApiClient.AOT.Task
{
    /// <summary>
    /// 表示接口
    /// </summary>
    class CeInterface : CeMetadata
    {
        /// <summary>
        /// 获取接口类型
        /// </summary>
        public TypeDefinition Type { get; private set; }

        /// <summary>
        /// 表示接口
        /// </summary>
        /// <param name="interface">接口类型</param>
        public CeInterface(TypeDefinition @interface)
            : base(@interface.Module)
        {
            this.Type = @interface;
        }

        /// <summary>
        /// 返回是否为继承IHttpApi的接口
        /// </summary>
        /// <returns></returns>
        public bool IsHttpApiInterface()
        {
            if (this.Type.IsInterface == false)
            {
                return false;
            }
            return this.Type.Interfaces.Any(i => this.TypeReferenceEquals(i.InterfaceType, typeof(IHttpApi)));
        }

        /// <summary>
        /// 生成对应的代理类型
        /// </summary>
        /// <param name="prefix">类型名称前缀</param>
        /// <returns></returns>
        public TypeDefinition MakeProxyType(string prefix = "$")
        {
            var @namespace = this.Type.Namespace;
            var proxyTypeName = $"{prefix}{this.Type.Name}";
            var classAttributes = this.GetProxyTypeAttributes();
            var baseType = this.GetTypeReference(typeof(HttpApiClient));

            var proxyType = new TypeDefinition(@namespace, proxyTypeName, classAttributes, baseType)
            {
                DeclaringType = this.Type.DeclaringType
            };

            // 继承的接口，泛型接口就声明泛型类
            // 由于mono.cecil的bug，无法获得泛型接口的泛型参数约束
            // 所以也就无法支持泛型约束的接口声明的代理类的生成
            if (this.Type.HasGenericParameters == true)
            {
                var genericParameter = this.Type
                    .GenericParameters
                    .Select(p => new GenericParameter(p.Name, proxyType))
                    .ToArray();

                var genericInterface = new GenericInstanceType(this.Type);
                foreach (var arg in genericParameter)
                {
                    proxyType.GenericParameters.Add(arg);
                    genericInterface.GenericArguments.Add(arg);
                }
                proxyType.Interfaces.Add(new InterfaceImplementation(genericInterface));
            }
            else
            {
                proxyType.Interfaces.Add(new InterfaceImplementation(this.Type));
            }
            return proxyType;
        }

        /// <summary>
        /// 返回代理类型的可见性
        /// </summary>
        /// <returns></returns>
        private TypeAttributes GetProxyTypeAttributes()
        {
            if (this.Type.IsNestedPrivate)
            {
                return TypeAttributes.NestedPrivate | TypeAttributes.BeforeFieldInit;
            }
            if (this.Type.IsNestedAssembly)
            {
                return TypeAttributes.NestedAssembly | TypeAttributes.BeforeFieldInit;
            }
            if (this.Type.IsNestedPublic)
            {
                return TypeAttributes.NestedPublic | TypeAttributes.BeforeFieldInit;
            }
            if (this.Type.IsPublic)
            {
                return TypeAttributes.Public;
            }
            return TypeAttributes.NotPublic;
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
            if (method.HasGenericParameters == true)
            {
                throw new NotSupportedException($"不支持泛型方法：{method}");
            }

            if (method.IsSpecialName == true)
            {
                throw new NotSupportedException($"不支持属性访问器：{method}");
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
        /// TypeDefinition比较器
        /// </summary>
        private class TypeDefinitionComparer : IEqualityComparer<TypeDefinition>
        {
            /// <summary>
            /// 获取唯一实例
            /// </summary>
            public static readonly TypeDefinitionComparer Instance = new TypeDefinitionComparer();

            /// <summary>
            /// 是否相等
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(TypeDefinition x, TypeDefinition y)
            {
                return true;
            }

            /// <summary>
            /// 返回哈希值
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode(TypeDefinition obj)
            {
                return obj.FullName.GetHashCode();
            }
        }
    }
}
