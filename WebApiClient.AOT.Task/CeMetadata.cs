using Mono.Cecil;
using System;
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
        /// 所有已知类型
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
        /// 返回的导入后类型
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        protected TypeReference ImportType<T>()
        {
            var type = typeof(T);
            if (type.IsArray == true)
            {
                throw new NotSupportedException("不支持数组类型");
            }

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
        /// 返回导入类型的指定方法的方法
        /// </summary>
        /// <param name="methodName">方法名</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        protected MethodReference ImportMethod<T>(string methodName)
        {
            return this.ImportMethod<T>(item => item.Name == methodName);
        }

        /// <summary>
        /// 返回导入类型的指定方法的方法
        /// </summary>
        /// <param name="filter">方法过滤器</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentException"></exception>
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
                throw new ArgumentException("无法找到指定的方法");
            }
            return this.module.ImportReference(method);
        }

        /// <summary>
        /// 比较两类型类型是一样
        /// </summary>
        /// <param name="source">类型</param>
        /// <param name="target">目标类型</param>
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
    }
}
