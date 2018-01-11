using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示将参数值作为application/json请求
    /// 每个Api只能注明于其中的一个参数
    /// 依赖于HttpApiConfig.JsonFormatter
    /// </summary>
    public class JsonContentAttribute : HttpContentAttribute
    {
        /// <summary>
        /// 日期时间格式
        /// </summary>
        private readonly string datetimeFormat;

        /// <summary>
        /// 将参数值作为application/json请求
        /// </summary>
        public JsonContentAttribute()
            : this(null)
        {
        }

        /// <summary>
        /// 将参数体作为application/json请求
        /// </summary>
        /// <param name="datetimeFormat">日期时间格式</param>
        /// <exception cref="ArgumentNullException"></exception>
        public JsonContentAttribute(string datetimeFormat)
        {
            this.datetimeFormat = datetimeFormat;
        }

        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        protected override void SetHttpContent(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var formatter = context.HttpApiConfig.JsonFormatter;
            var timeFormat = context.HttpApiConfig.SelectDateTimeFormat(this.datetimeFormat);
            var content = formatter.Serialize(parameter.Value, timeFormat);
            context.RequestMessage.Content = new StringContent(content, Encoding.UTF8, "application/json");
        }
    }
}
