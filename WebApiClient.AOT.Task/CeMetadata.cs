using Mono.Cecil;
using System;
using System.Reflection;
using System.Linq;

namespace WebApiClient.AOT.Task
{
    /// <summary>
    /// 表示cecil元数据抽象
    /// </summary>
    abstract class CeMetadata
    {
        /// <summary>
        /// 所在程序集
        /// </summary>
        private readonly ModuleDefinition module;

        /// <summary>
        /// 获取所有已知类型
        /// </summary>
        private readonly TypeDefinition[] knowTypes;

        /// <summary>
        /// 获取系统类型
        /// </summary>
        public TypeSystem TypeSystem { get; private set; }

        /// <summary>
        /// cecil元数据抽象
        /// </summary>
        /// <param name="module">所在程序集</param>
        public CeMetadata(CeAssembly assembly)
        {
            this.module = assembly.MainMdule;
            this.knowTypes = assembly.KnowTypes;
            this.TypeSystem = this.module.TypeSystem;
        }

        /// <summary>
        /// 返回的导入类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        protected TypeReference ImportTypeReference(Type type)
        {
            var knowType = this.knowTypes.FirstOrDefault(item => item.FullName == type.FullName);
            if (knowType == null)
            {
                return this.module.ImportReference(type);
            }
            else
            {
                return this.module.ImportReference(knowType);
            }
        }

        /// <summary>
        /// 导入类型的指定方法
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="methodName">方法名</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        protected MethodReference ImportMethodReference(Type type, string methodName)
        {
            var typeReference = this.ImportTypeReference(type);
            return this.ImportMethodReference(typeReference, methodName);
        }

        /// <summary>
        /// 导入类型的指定方法
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="methodName">方法名</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        protected MethodReference ImportMethodReference(TypeReference type, string methodName)
        {
            var method = type.Resolve().Methods.FirstOrDefault(item => item.Name == methodName);
            if (method == null)
            {
                throw new ArgumentException(nameof(methodName));
            }
            return this.module.ImportReference(method);
        }

        /// <summary>
        /// 比较类型是否相等
        /// </summary>
        /// <param name="typeReference"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected bool TypeReferenceEquals(TypeReference typeReference, Type type)
        {
            return typeReference.FullName == type.FullName;
        }
    }
}
