using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        /// path是否使用骆驼命名
        /// </summary>
        private readonly bool camelCasePath;

        /// <summary>
        /// 将自身作为JsonPatch请头内容
        /// </summary>
        /// <param name="camelCasePath">path是否使用骆驼命名</param>
        public JsonPatchDocument(bool camelCasePath = false)
        {
            this.camelCasePath = camelCasePath;
        }

        /// <summary>
        /// Replace操作
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="pathSeletor">path选择器</param>
        /// <param name="value">替换成的值</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Replace<TField>(Expression<Func<T, TField>> pathSeletor, TField value)
        {
            var path = this.GetExpressionPath(pathSeletor);
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
            var path = this.GetExpressionPath(pathSeletor);
            base.Remove(path);
        }

        /// <summary>
        /// 返回表示式对应的path
        /// </summary>
        /// <param name="pathSeletor">path选择器</param>
        /// <returns></returns>
        private string GetExpressionPath(LambdaExpression pathSeletor)
        {
            var visitor = new PathVisitor(pathSeletor, this.GetMemberName, this.camelCasePath);
            return visitor.ToString();
        }

        /// <summary>
        /// 返回成员的名称
        /// </summary>
        /// <param name="member">成员</param>
        /// <returns></returns>
        protected virtual string GetMemberName(MemberInfo member)
        {
            var aliasAs = member.GetCustomAttribute<AliasAsAttribute>();
            if (aliasAs != null && aliasAs.IsDefinedScope(FormatScope.JsonFormat))
            {
                return aliasAs.Name;
            }
            var jsonProperty = member.GetCustomAttribute<JsonPropertyAttribute>();
            if (jsonProperty != null)
            {
                return jsonProperty.PropertyName;
            }
            return member.Name;
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
            /// 成员名称委托
            /// </summary>
            private readonly Func<MemberInfo, string> nameFunc;

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
            /// <param name="nameFunc">成员名称委托</param>
            /// <param name="camelCasePath">path是否使用骆驼命名</param>
            /// <exception cref="ArgumentNullException"></exception>
            public PathVisitor(LambdaExpression pathSeletor, Func<MemberInfo, string> nameFunc, bool camelCasePath)
            {
                if (pathSeletor == null)
                {
                    throw new ArgumentNullException(nameof(pathSeletor));
                }
                this.nameFunc = nameFunc;
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
                var name = cache.GetOrAdd(node.Member, m => this.nameFunc(m));
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
                if (node.NodeType == ExpressionType.ArrayIndex)
                {
                    if (node.Right.NodeType == ExpressionType.Constant)
                    {
                        var index = node.Right.ToString();
                        this.path.Insert(0, $"/{index}");
                    }
                    else
                    {
                        var body = Expression.Convert(node.Right, typeof(object));
                        var index = Expression.Lambda<Func<object>>(body).Compile().Invoke();
                        this.path.Insert(0, $"/{index}");
                    }
                }
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
