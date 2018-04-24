using System;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Parameterables
{
    /// <summary>
    /// 表示将自身作为请求的授权
    /// </summary>
    public class Authorization : IApiParameterable
    {
        /// <summary>
        /// 体系
        /// </summary>
        public string Scheme { get; private set; }

        /// <summary>
        /// 参数
        /// </summary>
        public string Parameter { get; private set; }

        /// <summary>
        /// 授权信息
        /// </summary>
        /// <param name="scheme">体系</param>
        /// <param name="parameter">参数</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Authorization(string scheme, string parameter)
        {
            if (string.IsNullOrEmpty(scheme))
            {
                throw new ArgumentNullException(nameof(scheme));
            }
            if (string.IsNullOrEmpty(parameter))
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            this.Scheme = scheme;
            this.Parameter = parameter;
        }

        /// <summary>
        /// 执行请求前
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public Task BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            const string headerName = "Authorization";
            var header = context.RequestMessage.Headers;
            header.Remove(headerName);
            header.TryAddWithoutValidation(headerName, this.GetAuthorizationValue());
            return ApiTask.CompletedTask;
        }

        /// <summary>
        /// 返回授权信息
        /// </summary>
        /// <returns></returns>
        private string GetAuthorizationValue()
        {
            return $"{this.Scheme} {this.Parameter}";
        }

        /// <summary>
        /// 转换为文本
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.GetAuthorizationValue();
        }
    }
}
