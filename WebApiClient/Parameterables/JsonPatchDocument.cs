using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;
using WebApiClient.DataAnnotations;

namespace WebApiClient.Parameterables
{
    /// <summary>
    /// 表示将自身作为JsonPatch请头内容
    /// </summary>
    public class JsonPatchDocument : IApiParameterable
    {
        /// <summary>
        /// 操作列表
        /// </summary>
        private readonly List<object> oprations = new List<object>();

        /// <summary>
        /// Add操作
        /// </summary>
        /// <param name="path">json路径</param>
        /// <param name="value">值</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Add(string path, object value)
        {
            if (string.IsNullOrEmpty(path) == true)
            {
                throw new ArgumentNullException(nameof(path));
            }

            this.oprations.Add(new
            {
                op = "add",
                path,
                value
            });
        }

        /// <summary>
        /// Remove操作
        /// </summary>
        /// <param name="path">json路径</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Remove(string path)
        {
            if (string.IsNullOrEmpty(path) == true)
            {
                throw new ArgumentNullException(nameof(path));
            }

            this.oprations.Add(new
            {
                op = "remove",
                path
            });
        }

        /// <summary>
        /// Replace操作
        /// </summary>
        /// <param name="path">json路径</param>
        /// <param name="value">替换后的值</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Replace(string path, object value)
        {
            if (string.IsNullOrEmpty(path) == true)
            {
                throw new ArgumentNullException(nameof(path));
            }

            this.oprations.Add(new
            {
                op = "replace",
                path,
                value
            });
        }

        /// <summary>
        /// 执行请求前
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task IApiParameterable.BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var formatter = context.HttpApiConfig.JsonFormatter;
            var options = context.HttpApiConfig.FormatOptions;
            var json = formatter.Serialize(this.oprations, options);

            context.RequestMessage.Content = new JsonPatchContent(json, Encoding.UTF8);
            return ApiTask.CompletedTask;
        }
    }

    /// <summary>
    /// 表示将自身作为JsonPatch请头内容
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonPatchDocument<T> : JsonPatchDocument where T : class
    {
        /// <summary>
        /// 实例
        /// </summary>
        private readonly T instance;

        /// <summary>
        /// path是否使用骆驼命名
        /// </summary>
        private readonly bool camelCasePath;

        /// <summary>
        /// 将自身作为JsonPatch请头内容
        /// </summary>
        /// <param name="instance">变更后的实例</param>
        /// <param name="camelCasePath">path是否使用骆驼命名</param>
        /// <exception cref="ArgumentNullException"></exception>
        public JsonPatchDocument(T instance, bool camelCasePath = false)
        {
            this.instance = instance ?? throw new ArgumentNullException(nameof(instance));
            this.camelCasePath = camelCasePath;
        }

        /// <summary>
        /// Replace操作
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="pathSeletor">path选择器</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Replace<TField>(Expression<Func<T, TField>> pathSeletor)
        {
            var path = new PathVisitor(pathSeletor, this.camelCasePath).ToString();
            var value = pathSeletor.Compile().Invoke(this.instance);
            base.Replace(path, value);
        }

        /// <summary>
        /// Remove操作
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="pathSeletor">path选择器</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Remove<TField>(Expression<Func<T, TField>> pathSeletor)
        {
            var path = new PathVisitor(pathSeletor, this.camelCasePath).ToString();
            base.Remove(path);
        }

        /// <summary>
        /// 表示Path访问器
        /// </summary>
        class PathVisitor : ExpressionVisitor
        {
            /// <summary>
            /// path是否使用骆驼命名
            /// </summary>
            private readonly bool camelCasePath;

            /// <summary>
            /// path变量
            /// </summary>
            private readonly StringBuilder path = new StringBuilder();

            /// <summary>
            /// 缓存
            /// </summary>
            private static readonly ConcurrentCache<MemberInfo, string> cache = new ConcurrentCache<MemberInfo, string>();

            /// <summary>
            /// Path访问器
            /// </summary>
            /// <param name="pathSeletor">表达式</param>
            /// <param name="camelCasePath">path是否使用骆驼命名</param>
            /// <exception cref="ArgumentNullException"></exception>
            public PathVisitor(LambdaExpression pathSeletor, bool camelCasePath)
            {
                if (pathSeletor == null)
                {
                    throw new ArgumentNullException(nameof(pathSeletor));
                }

                this.camelCasePath = camelCasePath;
                base.Visit(pathSeletor.Body);
            }

            /// <summary>
            /// 访问成员时
            /// </summary>
            /// <param name="node"></param>
            /// <returns></returns>
            protected override Expression VisitMember(MemberExpression node)
            {
                var name = cache.GetOrAdd(node.Member, m =>
                {
                    var aliasAs = m.GetCustomAttribute<AliasAsAttribute>();
                    return aliasAs == null ? m.Name : aliasAs.Name;
                });

                if (this.camelCasePath == true)
                {
                    name = FormatOptions.CamelCase(name);
                }

                this.path.Insert(0, $"/{name}");
                return base.VisitMember(node);
            }

            /// <summary>
            /// 访问二元表达式
            /// </summary>
            /// <param name="node"></param>
            /// <returns></returns>
            protected override Expression VisitBinary(BinaryExpression node)
            {
                var index = node.Right.ToString();
                this.path.Insert(0, $"/{index}");
                return base.VisitBinary(node);
            }

            /// <summary>
            /// 转换为字符串
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return this.path.ToString();
            }
        }
    }
}
