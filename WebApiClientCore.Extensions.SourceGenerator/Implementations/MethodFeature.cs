using System;
using System.Linq;
using System.Reflection;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 表示MethodInfo的特征
    /// </summary>
    sealed class MethodFeature : IEquatable<MethodFeature>
    {
        private readonly MethodInfo method;

        /// <summary>
        /// MethodInfo的特征
        /// </summary>
        /// <param name="method"></param>
        public MethodFeature(MethodInfo method)
        {
            this.method = method;
        }

        /// <summary>
        /// 比较方法原型是否相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MethodFeature other)
        {
            var x = this.method;
            var y = other.method;

            if (x.Name != y.Name || x.ReturnType != y.ReturnType)
            {
                return false;
            }

            var xParameterTypes = x.GetParameters().Select(p => p.ParameterType);
            var yParameterTypes = y.GetParameters().Select(p => p.ParameterType);
            return xParameterTypes.SequenceEqual(yParameterTypes);
        }

        /// <summary>
        /// 获取哈希
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(this.method.Name);
            hashCode.Add(this.method.ReturnType);
            foreach (var parameter in this.method.GetParameters())
            {
                hashCode.Add(parameter.ParameterType);
            }
            return hashCode.ToHashCode();
        }
    }
}
