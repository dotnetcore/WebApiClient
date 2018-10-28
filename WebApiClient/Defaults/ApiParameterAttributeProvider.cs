using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using WebApiClient.Attributes;

namespace WebApiClient.Defaults
{
    /// <summary>
    /// 表示默认的Api参数修饰特性提供者的行为
    /// </summary>
    public class ApiParameterAttributeProvider : IApiParameterAttributeProvider
    {
        /// <summary>
        /// 返回参数特性
        /// </summary>
        /// <param name="parameterType">参数类型</param>
        /// <param name="defined">参数上声明的特性</param>
        /// <returns></returns>
        public virtual IEnumerable<IApiParameterAttribute> GetAttributes(Type parameterType, IEnumerable<IApiParameterAttribute> defined)
        {
            var attributes = new ParameterAttributeCollection(defined);
            var isHttpContent = parameterType.IsInheritFrom<HttpContent>();
            var isApiParameterable = parameterType.IsInheritFrom<IApiParameterable>() || parameterType.IsInheritFrom<IEnumerable<IApiParameterable>>();

            if (isApiParameterable == true)
            {
                attributes.Add(new ParameterableAttribute());
            }
            else if (isHttpContent == true)
            {
                attributes.AddIfNotExists(new HttpContentAttribute());
            }
            else if (parameterType == typeof(CancellationToken))
            {
                attributes.Add(new CancellationTokenAttribute());
            }
            else if (attributes.Count == 0)
            {
                attributes.Add(new PathQueryAttribute());
            }
            return attributes;
        }

        /// <summary>
        /// 表示参数特性集合
        /// </summary>
        private class ParameterAttributeCollection : IEnumerable<IApiParameterAttribute>
        {
            /// <summary>
            /// 特性列表
            /// </summary>
            private readonly List<IApiParameterAttribute> attribueList = new List<IApiParameterAttribute>();

            /// <summary>
            /// 获取元素数量
            /// </summary>
            public int Count
            {
                get
                {
                    return this.attribueList.Count;
                }
            }

            /// <summary>
            /// 参数特性集合
            /// </summary>
            /// <param name="defined">声明的特性</param>
            public ParameterAttributeCollection(IEnumerable<IApiParameterAttribute> defined)
            {
                this.attribueList.AddRange(defined);
            }

            /// <summary>
            /// 添加新特性
            /// </summary>
            /// <param name="attribute">新特性</param>
            public void Add(IApiParameterAttribute attribute)
            {
                this.attribueList.Add(attribute);
            }

            /// <summary>
            /// 添加新特性
            /// </summary>
            /// <param name="attribute">新特性</param>
            /// <returns></returns>
            public bool AddIfNotExists(IApiParameterAttribute attribute)
            {
                var type = attribute.GetType();
                if (this.attribueList.Any(item => item.GetType() == type) == true)
                {
                    return false;
                }

                this.attribueList.Add(attribute);
                return true;
            }

            /// <summary>
            /// 转换为数组
            /// </summary>
            /// <returns></returns>
            public IApiParameterAttribute[] ToArray()
            {
                return this.attribueList.ToArray();
            }

            /// <summary>
            /// 返回迭代器
            /// </summary>
            /// <returns></returns>
            public IEnumerator<IApiParameterAttribute> GetEnumerator()
            {
                return this.attribueList.GetEnumerator();
            }

            /// <summary>
            /// 返回迭代器
            /// </summary>
            /// <returns></returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}
