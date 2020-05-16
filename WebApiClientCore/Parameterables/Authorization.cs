using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebApiClientCore.Parameterables
{
    /// <summary>
    /// 表示将自身作为请求的授权
    /// </summary>
    public class Authorization : IApiParameterable
    {
        /// <summary>
        /// 授权信息
        /// </summary>
        private readonly AuthenticationHeaderValue authentication;

        /// <summary>
        /// 体系
        /// </summary>
        public string Scheme => this.authentication.Scheme;

        /// <summary>
        /// 参数
        /// </summary>
        public string Parameter => this.authentication.Parameter;

        /// <summary>
        /// 授权信息
        /// </summary>
        /// <param name="scheme">体系</param>
        /// <param name="parameter">参数</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Authorization(string scheme, string parameter)
        {
            this.authentication = new AuthenticationHeaderValue(scheme, parameter);
        }

        /// <summary>
        /// 执行请求前
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task OnRequestAsync(ApiParameterContext context)
        {
            context.HttpContext.RequestMessage.Headers.Authorization = this.authentication;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 转换为文本
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.authentication.ToString();
        }
    }
}
