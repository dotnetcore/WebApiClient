using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;
using WebApiClientCore.HttpContents;

namespace WebApiClientCore.Parameters
{
    /// <summary>
    /// 表示将自身作为JsonPatch请求内容
    /// </summary>
    [DebuggerTypeProxy(typeof(DebugView))]
    public class JsonPatchDocument : IApiParameter
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
        public void Add(string path, object? value)
        {
            if (string.IsNullOrEmpty(path) == true)
            {
                throw new ArgumentNullException(nameof(path));
            }
            this.oprations.Add(new { op = "add", path, value });
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
            this.oprations.Add(new { op = "remove", path });
        }

        /// <summary>
        /// Replace操作
        /// </summary>
        /// <param name="path">json路径</param>
        /// <param name="value">替换后的值</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Replace(string path, object? value)
        {
            if (string.IsNullOrEmpty(path) == true)
            {
                throw new ArgumentNullException(nameof(path));
            }
            this.oprations.Add(new { op = "replace", path, value });
        }

        /// <summary>
        /// 执行请求前
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task OnRequestAsync(ApiParameterContext context)
        {
            if (context.HttpContext.RequestMessage.Method != HttpMethod.Patch)
            {
                throw new ApiInvalidConfigException(Resx.required_PatchMethod);
            }

            var jsonPatchContent = new JsonPatchContent();
            context.HttpContext.RequestMessage.Content = jsonPatchContent;

            var options = context.HttpContext.HttpApiOptions.JsonSerializeOptions;
            var serializer = context.HttpContext.ServiceProvider.GetJsonSerializer();
            serializer.Serialize(jsonPatchContent, this.oprations, options);

            return Task.CompletedTask;
        }

        /// <summary>
        /// 调试视图
        /// </summary>
        private class DebugView
        {
            /// <summary>
            /// 调试的目标
            /// </summary>
            private readonly JsonPatchDocument target;

            /// <summary>
            /// 调试视图
            /// </summary>
            /// <param name="target">查看的对象</param>
            public DebugView(JsonPatchDocument target)
            {
                this.target = target;
            }

            /// <summary>
            /// 查看的内容
            /// </summary>
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public List<object> Oprations
            {
                get => this.target.oprations;
            }
        }
    }
}
