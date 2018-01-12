
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示将参数值作为x-www-form-urlencoded请求
    /// 支持单一值类型如string、int、guid、枚举等，以及他们的可空类型或集合
    /// 支持POCO类型、IDictionaryOf(string,string)类型、IDictionaryOf(string,object)类型
    /// 依赖于HttpApiConfig.KeyValueFormatter
    /// </summary>
    public class FormContentAttribute : HttpContentAttribute
    {
        /// <summary>
        /// 时期时间格式
        /// </summary>
        private readonly string datetimeFormat;

        /// <summary>
        /// 将参数值作为x-www-form-urlencoded请求
        /// </summary>
        public FormContentAttribute()
            : this(null)
        {
        }

        /// <summary>
        /// 将参数值作为x-www-form-urlencoded请求
        /// </summary>
        /// <param name="datetimeFormat">时期时间格式</param>
        public FormContentAttribute(string datetimeFormat)
        {
            this.datetimeFormat = datetimeFormat;
        }

        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        protected override async Task SetHttpContentAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var formatter = context.HttpApiConfig.KeyValueFormatter;
            var options = context.HttpApiConfig.FormatOptions.CloneChange(this.datetimeFormat);
            var keyValues = formatter.Serialize(parameter, options);
            await context.RequestMessage.AddFormFieldAsync(keyValues);
        }
    }
}
