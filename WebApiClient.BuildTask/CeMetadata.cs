using Mono.Cecil;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WebApiClient.BuildTask
{
    /// <summary>
    /// Represents cecil metadata abstraction
    /// </summary>
    abstract class CeMetadata
    {
        /// <summary>
        /// The assembly
        /// </summary>
        private readonly ModuleDefinition module;

        /// <summary>
        /// All known types
        /// </summary>
        private readonly TypeDefinition[] knowTypes;

        /// <summary>
        /// Get system type
        /// </summary>
        public TypeSystem TypeSystem { get; private set; }

        /// <summary>
        /// cecil metadata abstraction
        /// </summary>
        /// <param name="module">The assembly</param>
        public CeMetadata(CeAssembly assembly)
        {
            this.module = assembly.MainMdule;
            this.knowTypes = assembly.KnowTypes;
            this.TypeSystem = this.module.TypeSystem;
        }

        /// <summary>
        /// Returns the type after importing the external type
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        /// <returns></returns>
        protected TypeReference ImportType<T>()
        {
            var type = typeof(T);
            if (type.IsArray == true)
            {
                throw new NotSupportedException("Array type is not supported");
            }

            var knowType = this.knowTypes.FirstOrDefault(item => item.FullName == type.FullName);
            if (knowType == null)
            {
                // This assembly type is not directly imported
                if (this.IsThisAssemblyType(type) == true)
                {
                    throw new TypeLoadException($"No type found：{type.FullName}");
                }
                return this.module.ImportReference(type);
            }
            else
            {
                return this.module.ImportReference(knowType);
            }
        }


        /// <summary>
        /// Returns that the specified type is within the scope of this assembly
        /// </summary>
        /// <param name="type">Types of</param>
        /// <returns></returns>
        private bool IsThisAssemblyType(Type type)
        {
#if NETCOREAPP1_1
            return type.GetTypeInfo().Assembly == this.GetType().GetTypeInfo().Assembly;
#else
            return type.Assembly == this.GetType().Assembly;
#endif
        }


        /// <summary>
        /// Returns the method after importing the specified method of the external type
        /// </summary>
        /// <param name="methodName">Method name</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        /// <returns></returns>
        protected MethodReference ImportMethod<T>(string methodName)
        {
            return this.ImportMethod<T>(item => item.Name == methodName);
        }

        /// <summary>
        /// Returns the method after importing the specified method of the external type
        /// </summary>
        /// <param name="filter">Method filter</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        /// <returns></returns>
        protected MethodReference ImportMethod<T>(Func<MethodDefinition, bool> filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            var method = this.ImportType<T>().Resolve().Methods.FirstOrDefault(filter);
            if (method == null)
            {
                throw new ArgumentException("Cannot find the specified method");
            }
            return this.module.ImportReference(method);
        }

        /// <summary>
        /// Comparing two types is the same
        /// </summary>
        /// <param name="source">Types of</param>
        /// <param name="target">Target type</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        protected bool IsTypeEquals(TypeReference source, Type target)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            return source.FullName == target.FullName;
        }

        /// <summary>
        /// Returns the full name of the method
        /// </summary>
        /// <param name="method">method</param>
        /// <returns></returns>
        protected string GetMethodFullName(MethodReference method)
        {
            var builder = new StringBuilder();
            foreach (var p in method.Parameters)
            {
                if (builder.Length > 0)
                {
                    builder.Append(",");
                }
                builder.Append(GetTypeName(p.ParameterType));
            }
            var insert = $"{GetTypeName(method.ReturnType)} {method.Name}(";
            return builder.Insert(0, insert).Append(")").ToString();
        }

        /// <summary>
        /// Return type without namespace
        /// </summary>
        /// <param name="type">Types of</param>
        /// <returns></returns>
        private static string GetTypeName(TypeReference type)
        {
            if (type.IsGenericInstance == false)
            {
                return type.Name;
            }

            var builder = new StringBuilder();
            var parameters = ((GenericInstanceType)type).GenericArguments;
            foreach (var p in parameters)
            {
                if (builder.Length > 0)
                {
                    builder.Append(",");
                }
                builder.Append(GetTypeName(p));
            }

            return builder.Insert(0, $"{type.Name}<").Append(">").ToString();
        }
    }
}
