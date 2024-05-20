using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WebApiClientCore
{
    /// <summary>
    /// HttpApi的方法查找器
    /// </summary>
    static class HttpApiMethodFinder
    {
        /// <summary>
        /// 查找接口类型及其继承的接口的所有方法
        /// </summary>
        /// <param name="httpApiType">接口类型</param> 
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> FindApiMethods(Type httpApiType)
        {
            var interfaces = httpApiType.GetInterfaces().Append(httpApiType);
            return Sort(interfaces, t => t.GetInterfaces())
                .Reverse()
                .SelectMany(item => item.GetMethods().Select(m => new MethodFeature(m)))
                .Distinct()
                .Select(item => item.Method);
        }

        /// <summary>
        /// https://www.cnblogs.com/myzony/p/9201768.html
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="getDependencies"></param>
        /// <returns></returns>
        private static IList<T> Sort<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies)
        {
            var sorted = new List<T>();
            var visited = new Dictionary<T, bool>();

            foreach (var item in source)
            {
                Visit(item, getDependencies, sorted, visited);
            }

            return sorted;
        }

        private static void Visit<T>(T item, Func<T, IEnumerable<T>> getDependencies, List<T> sorted, Dictionary<T, bool> visited)
        {
            bool inProcess;
            var alreadyVisited = visited.TryGetValue(item, out inProcess);

            // 如果已经访问该顶点，则直接返回
            if (alreadyVisited)
            {
                // 如果处理的为当前节点，则说明存在循环引用
                if (inProcess)
                {
                    throw new ArgumentException("Cyclic dependency found.");
                }
            }
            else
            {
                // 正在处理当前顶点
                visited[item] = true;

                // 获得所有依赖项
                var dependencies = getDependencies(item);
                // 如果依赖项集合不为空，遍历访问其依赖节点
                if (dependencies != null)
                {
                    foreach (var dependency in dependencies)
                    {
                        // 递归遍历访问
                        Visit(dependency, getDependencies, sorted, visited);
                    }
                }

                // 处理完成置为 false
                visited[item] = false;
                sorted.Add(item);
            }
        }

        /// <summary>
        /// 表示MethodInfo的特征
        /// </summary>
        private class MethodFeature : IEquatable<MethodFeature>
        {
            private readonly MethodInfo method;

            public MethodInfo Method => method;

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

            public override bool Equals(object obj)
            {
                return obj is MethodFeature other && this.Equals(other);
            }
        }
    }
}
