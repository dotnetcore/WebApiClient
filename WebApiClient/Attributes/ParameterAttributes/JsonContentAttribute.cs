using System;
using System.Text;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 使用JsonFormatter序列化参数值得到的json文本作为application/json请求
    /// 每个Api只能注明于其中的一个参数
    /// </summary>
    public class JsonContentAttribute : HttpContentAttribute, IDateTimeFormatable, IEncodingable
    {
        /// <summary>
        /// 编码方式
        /// </summary>
        private Encoding encoding = System.Text.Encoding.UTF8;

        /// <summary>
        /// 获取或设置编码名称
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public string Encoding
        {
            get => this.encoding.WebName;
            set => this.encoding = System.Text.Encoding.GetEncoding(value);
        }

        /// <summary>
        /// 获取或设置时期时间格式
        /// </summary>
        public string DateTimeFormat { get; set; }

        /// <summary>
        /// 序列化参数值得到的json文本作为application/json请求
        /// </summary>
        public JsonContentAttribute()
        {
        }

        /// <summary>
        /// 序列化参数值得到的json文本作为application/json请求
        /// </summary>
        /// <param name="datetimeFormat">日期时间格式</param>
        public JsonContentAttribute(string datetimeFormat)
        {
            this.DateTimeFormat = datetimeFormat;
        }

        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        protected override void SetHttpContent(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var formatter = context.HttpApiConfig.JsonFormatter;
            var options = context.HttpApiConfig.FormatOptions.CloneChange(this.DateTimeFormat);
            var json = formatter.Serialize(parameter.Value, options);
            context.RequestMessage.Content = new JsonContent(json, this.encoding);
        }
    }
}
