using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiClient.BuildTask
{
    /// <summary>
    /// Represents the interface
    /// </summary>
    class CeInterface : CeMetadata
    {
        /// <summary>
        /// Get interface type
        /// </summary>
        public TypeDefinition Type { get; private set; }

        /// <summary>
        /// Represents the interface
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <param name="interface">Interface Type</param>
        public CeInterface(CeAssembly assembly, TypeDefinition @interface)
            : base(assembly)
        {
            this.Type = @interface;
        }

        /// <summary>
        /// Returns whether the interface inherits IHttpApi
        /// </summary>
        /// <returns></returns>
        public bool IsHttpApiInterface()
        {
            if (this.Type.IsInterface == false)
            {
                return false;
            }
            return this.Type.Interfaces.Any(i => this.IsTypeEquals(i.InterfaceType, typeof(IHttpApi)));
        }

        /// <summary>
        /// Generate the corresponding proxy type
        /// </summary>
        /// <param name="prefix">Type name prefix</param>
        /// <returns></returns>
        public TypeDefinition MakeProxyType(string prefix = "$")
        {
            var @namespace = this.Type.Namespace;
            var proxyTypeName = $"{prefix}{this.Type.Name}";
            var classAttributes = this.GetProxyTypeAttributes();
            var baseType = this.ImportType<HttpApi>();

            var proxyType = new TypeDefinition(@namespace, proxyTypeName, classAttributes, baseType)
            {
                DeclaringType = this.Type.DeclaringType
            };

            // Inherited interfaces, generic interfaces declare generic classes
            if (this.Type.HasGenericParameters == true)
            {
                var genericParameters = this.Type
                    .GenericParameters
                    .Select(p => this.MakeGenericParameter(p, proxyType))
                    .ToArray();

                var genericInterface = new GenericInstanceType(this.Type);
                foreach (var arg in genericParameters)
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
        /// Generating generic parameters for proxy classes
        /// </summary>
        /// <param name="proxyType">Proxy type</param>
        /// <returns></returns>
        private GenericParameter MakeGenericParameter(GenericParameter source, TypeDefinition proxyType)
        {
            var parameter = new GenericParameter(source.Name, proxyType)
            {
                Attributes = source.Attributes
                & ~GenericParameterAttributes.Covariant
                & ~GenericParameterAttributes.Contravariant
            };

            foreach (var type in source.Constraints)
            {
                parameter.Constraints.Add(type);
            }
            return parameter;
        }

        /// <summary>
        /// Return visibility of proxy type
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
        /// Get all methods of an interface type and its inherited interfaces
        /// Methods that ignore the IHttpApi interface
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public MethodDefinition[] GetAllApis()
        {
            var excepts = this.ImportType<HttpApi>()
                .Resolve()
                .Interfaces
                .Select(item => item.InterfaceType.Resolve());

            var interfaces = new[] { this.Type }.Concat(this.Type.Interfaces.Select(i => i.InterfaceType.Resolve()))
                .Except(excepts, TypeDefinitionComparer.Instance)
                .ToArray();

            var apiMethods = interfaces
                .SelectMany(item => item.Methods)
                .OrderBy(item => base.GetMethodFullName(item))
                .ToArray();

            foreach (var method in apiMethods)
            {
                this.EnsureApiMethod(method);
            }
            return apiMethods;
        }

        /// <summary>
        /// Make sure the method is a supported API interface
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        private void EnsureApiMethod(MethodDefinition method)
        {
            if (method.HasGenericParameters == true)
            {
                throw new NotSupportedException($"Does not support generic methods：{method}");
            }

            if (method.IsSpecialName == true)
            {
                throw new NotSupportedException($"Does not support property accessors：{method}");
            }

            var genericType = method.ReturnType;
            if (genericType.IsGenericInstance == true)
            {
                genericType = genericType.GetElementType();
            }

            var isTaskType = this.IsTypeEquals(genericType, typeof(Task<>)) || this.IsTypeEquals(genericType, typeof(ITask<>));
            if (isTaskType == false)
            {
                var message = $"The return type must be Task <> or ITask <>：{method}";
                throw new NotSupportedException(message);
            }

            foreach (var parameter in method.Parameters)
            {
                if (parameter.ParameterType.IsByReference == true)
                {
                    var message = $"Interface parameters do not support ref / out decoration：{parameter}";
                    throw new NotSupportedException(message);
                }
            }
        }

        /// <summary>
        /// TypeDefinition comparator
        /// </summary>
        private class TypeDefinitionComparer : IEqualityComparer<TypeDefinition>
        {
            /// <summary>
            /// Get unique instance
            /// </summary>
            public static readonly TypeDefinitionComparer Instance = new TypeDefinitionComparer();

            /// <summary>
            /// Are they equal
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(TypeDefinition x, TypeDefinition y)
            {
                return true;
            }

            /// <summary>
            /// Returns the hash value
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
