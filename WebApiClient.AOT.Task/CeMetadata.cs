using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient.AOT.Task
{
    /// <summary>
    /// 表示cecil元数据抽象
    /// </summary>
    abstract class CeMetadata
    {
        /// <summary>
        /// 所在模块
        /// </summary>
        private readonly ModuleDefinition module;

        /// <summary>
        /// cecil元数据抽象
        /// </summary>
        /// <param name="module">所在模块</param>
        public CeMetadata(ModuleDefinition module)
        {
            this.module = module;
        }

        /// <summary>
        /// 返回TypeReference
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        protected TypeReference GetTypeReference(Type type)
        {
            return this.module.ImportReference(type);
        }

        /// <summary>
        /// 返回MethodReference
        /// </summary>
        /// <param name="method">方法</param>
        /// <returns></returns>
        protected MethodReference GetMethodReference(MethodBase method)
        {
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
            return typeReference.FullName == this.GetTypeReference(type).FullName;
        }
    }
}
