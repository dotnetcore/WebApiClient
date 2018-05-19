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
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected TypeReference ImportTypeReference<T>()
        {
            var type = typeof(T);
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
        /// <param name="methodName">方法名</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        protected MethodReference ImportMethodReference<T>(string methodName)
        {
            return this.ImportMethodReference<T>(item => item.Name == methodName);
        }

        /// <summary>
        /// 导入类型的指定方法
        /// </summary>
        /// <param name="filer">方法过滤器</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        protected MethodReference ImportMethodReference<T>(Func<MethodDefinition, bool> filer)
        {
            var typeReference = this.ImportTypeReference<T>();
            var method = typeReference.Resolve().Methods.FirstOrDefault(filer);

            if (method == null)
            {
                throw new ArgumentException("无法找到指定的方法");
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
